using Eletiva.BranchAndBound.Services;
using Eletiva.Simplex.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eletiva.Simplex
{
    public sealed class Simplex : IMaximizer
    {
        /// <summary>
        /// Classes usadas para montar a tabela
        /// BaseVariableValue: Valores centrais, convergem em Base X Variavel
        /// VariableValue: Valores da linha Z, que estão na ultima linha da tabela
        /// BaseValue: Valores da coluna b, que estão na ultima coluna da tabela
        /// TotalValor: Função Objetivo
        /// </summary>
        private BranchAndBound.Entities.Info _info;
        private List<BaseVariableValue> _baseVariableValue;
        private List<VariableValue> _variableValues;
        private List<BaseValue> _baseValues;
        private TotalValue _totalValue;

        public BranchAndBound.Entities.Result Execute(BranchAndBound.Entities.Info info)
        {
            Reset(info);
            FetchTable();
            Iteration();
            PrintResult();
            return ParseResult();
        }

        void Reset(BranchAndBound.Entities.Info info)
        {
            _info = info;
            _baseVariableValue = new List<BaseVariableValue>();
            _variableValues = new List<VariableValue>();
            _baseValues = new List<BaseValue>();
            _totalValue = new TotalValue(0);
        }

        #region Parse Result

        private BranchAndBound.Entities.Result ParseResult() => new BranchAndBound.Entities.Result
        {
            Z = _totalValue.Value,
            VariableResults = _info.Variables.Select(variable => new BranchAndBound.Entities.Result.VariableResult
            {
                Value = (_baseValues.Where(vv => vv.Base.Name.Equals(variable.Description)).FirstOrDefault()?.Value).GetValueOrDefault(),
                Variable = variable
            })
            .ToList()
        };

        #endregion

        #region Build Table

        /// <summary>
        /// Método que lê o arquivo e cria os objetos que representam a tabela
        /// </summary>
        private void FetchTable()
        {
            // Preenchendo valores centrais, convergem em Base X Variavel
            FetchVariableValues();
            // Preenchendo os valores da ultima coluna
            FetchBaseValues();
            // Preenchendo os valores Base X Variavel
            FetchBaseVariableValues();
            // Completando o resto
            FillRest();
            // Criando a função objetiva
            _totalValue = new TotalValue(0);
        }

        private void FillRest()
        {
            // Preencher os variaveis de apoio (no ex: x3, x4, x5)
            foreach (var baseValue in _baseValues)
            {
                var variableValue = new VariableValue(new Variable(baseValue.Base.Name, baseValue.Base.Index), 0);
                _variableValues.Add(variableValue);
                foreach (var baseValue2 in _baseValues)
                {
                    // Quando a variavel tiver o mesmo nome que a base, valor igual a 1
                    if (variableValue.Variable.Name.Equals(baseValue2.Base.Name))
                        _baseVariableValue.Add(new BaseVariableValue(baseValue2.Base, variableValue.Variable, 1));
                    else
                        _baseVariableValue.Add(new BaseVariableValue(baseValue2.Base, variableValue.Variable, 0));
                }
            }
        }

        private void FetchBaseVariableValues()
        {
            // Pular a primeira linha do txt e ler as proximas que são as regras e as bases
            for (int count = 0; count < _baseValues.Count(); count++)
            {
                var baseValue = _baseValues[count];
                for (int variableCount = 0; variableCount < _variableValues.Count(); variableCount++)
                {
                    var variableValue = _variableValues[variableCount];
                    var value = _info
                        .Restrictions
                        .ElementAt(count)
                        .VariableValues
                        .Where(vv => variableValue.Variable.Name.Equals(vv.Variable.Description))
                        .First()
                        .Value;
                    _baseVariableValue.Add(new BaseVariableValue(baseValue.Base, variableValue.Variable, value));
                }
            }
        }

        private void FetchBaseValues()
        {
            var maxCount = _variableValues.Max(v => v.Variable.Index) + 1;
            for (int count = 0; count < _info.Restrictions.Count(); count++)
            {
                var restriction = _info.Restrictions.ElementAt(count);
                var index = count + maxCount;
                var @base = new Base($"x{index}", index);
                _baseValues.Add(new BaseValue(@base, restriction.Value));
            }
        }

        private void FetchVariableValues()
        {
            for (int count = 0; count < _info.Rules.Count(); count++)
            {
                var variableValue = _info.Rules.ElementAt(count);
                var variable = new Variable(variableValue.Variable.Description, count + 1); ;
                _variableValues.Add(new VariableValue(variable, variableValue.Value * -1));
            }
        }

        #endregion

        #region Iteration

        /// <summary>
        /// Inicia a iteração e verifica se a linha Z tem algum valor menor que 0
        /// Se sim, sai da função pois foi finalizada
        /// </summary>
        private void Iteration()
        {
            while (_variableValues.Any(v => v.Value < 0M))
            {
                // Pega a variavel que vai tombar para a base
                var variableToEnter = GetVariableThatWillEnter();
                // Pega a base que vai sair
                var baseToChange = GetBaseThatComeOut(variableToEnter);
                // Troca a base com a variavel
                baseToChange.ChangeBase(variableToEnter);
                // Começa o ajuste dos valores da tabela
                Calculate(baseToChange, variableToEnter);
            }
        }

        private void Calculate(Base @base, Variable variable)
        {
            // Primeiro ajusta a linha central, deixando o Valor central como 1 e ajustando as outras colunas
            var centerValue = GetBaseVariableValue(@base, variable);
            var value = 1 / centerValue.Value;
            var baseVariableValues = _baseVariableValue.Where(bv => @base.Id.Equals(bv.Base.Id)).ToList();
            baseVariableValues.ForEach(baseVariableValue => baseVariableValue.ChangeValue(baseVariableValue.Value * value));
            var baseValue = _baseValues.First(b => @base.Id.Equals(b.Base.Id));
            baseValue.ChangeValue(baseValue.Value * value);

            // Começa o ajuste das outras colunas, para zera-las, passando a base, variavel, e a linha da base
            CalculateOtherLines(@base, variable, baseVariableValues);
        }

        private void CalculateOtherLines(Base @base, Variable variable, List<BaseVariableValue> baseLine)
        {
            var centerValue = GetBaseVariableValue(@base, variable);
            var otherBases = _baseVariableValue.Except(baseLine).ToList();
            var centers = otherBases.Where(bv => variable.Id.Equals(bv.Variable.Id)).Select(bv => bv).ToList();
            centers.ForEach(center =>
            {
                // Calcula os valores são Base X Variavel e a coluna "b" (ultima coluna)
                CaculateBaseVariableValuesAndBaseValues(baseLine, center, centerValue, @base);
                // Calcula a linha de Z e o valor objetivo de Z
                CalculateVariableValuesAndTotalValue(variable, baseLine, centerValue);
            });
        }

        private void CaculateBaseVariableValuesAndBaseValues(List<BaseVariableValue> baseLine, BaseVariableValue center, BaseVariableValue centerValue, Base @base)
        {
            // Descobre o valor para multiplicar e faz outra linha menos esse valor, Passo 6 (Faz para valores Base X Variavel e Coluna b)
            var value = center.Value / centerValue.Value;
            baseLine.ForEach(bv =>
            {
                var valueToSubtract = bv.Value * value;
                var baseVariableValue = GetBaseVariableValue(center.Base, bv.Variable);
                baseVariableValue.ChangeValue(baseVariableValue.Value - valueToSubtract);
            });
            var baseValue = _baseValues.Where(b => @base.Id.Equals(b.Base.Id)).First();
            var valueToSubtract = baseValue.Value * value;
            var baseValueToChange = _baseValues.Where(b => center.Base.Id.Equals(b.Base.Id)).First();
            baseValueToChange.ChangeValue(baseValueToChange.Value - valueToSubtract);
        }

        private void CalculateVariableValuesAndTotalValue(Variable variable, List<BaseVariableValue> baseLine, BaseVariableValue centerValue)
        {
            // Descobre o valor para multiplicar e faz outra linha menos esse valor, Passo 6 (Faz para Linha Z e Valor Total)
            var variableValue = _variableValues.Where(v => variable.Id.Equals(v.Variable.Id)).First();
            var value = variableValue.Value / centerValue.Value;
            baseLine.ForEach(bv =>
            {
                var valueToSubtract = bv.Value * value;
                var variableValue = _variableValues.Where(v => bv.Variable.Id.Equals(v.Variable.Id)).First();
                variableValue.ChangeValue(variableValue.Value - valueToSubtract);
            });
            var baseValue = _baseValues.Where(b => centerValue.Base.Id.Equals(b.Base.Id)).First();
            var valueToSubtract = baseValue.Value * value;
            _totalValue.ChangeValue(_totalValue.Value - valueToSubtract);
        }

        private BaseVariableValue GetBaseVariableValue(Base @base, Variable variable) =>
            _baseVariableValue.First(value => @base.Id.Equals(value.Base.Id) && variable.Id.Equals(value.Variable.Id));

        private Base GetBaseThatComeOut(Variable variable)
        {
            // Pega a base que vai sair dividindo o Valor pelo Base X Variavel central e pegando a base que tem menor valor na Coluna b
            var lowerRatio = _baseValues.Select(baseValue =>
            {
                var baseVariableValue = GetBaseVariableValue(baseValue.Base, variable);
                return (0M.Equals(baseVariableValue.Value) ? 0M : baseValue.Value / baseVariableValue.Value, baseValue.Base);
            })
            .Where(l => l.Item1 != 0M)
            .ToList();
            var minValue = lowerRatio.Min(l => l.Item1);
            return lowerRatio.First(l => minValue.Equals(l.Item1)).Base;
        }

        private Variable GetVariableThatWillEnter()
        {
            // Pega a variavel que tem menor valor na linha Z
            var minVariableValue = _variableValues.Min(variableValue => variableValue.Value);
            var variable = _variableValues.Where(variableValue => minVariableValue.Equals(variableValue.Value)).First().Variable;
            return variable;
        }

        #endregion

        #region Print Result

        /// <summary>
        /// Método que mostra o resultado
        /// </summary>
        private void PrintResult()
        {
            Console.WriteLine("\nSIMPLEX");
            var variables = _variableValues.Select(v => v.Variable.Name);
            var basics = _baseValues.Select(v => v.Base.Name);
            var notBasic = string.Join(" = ", variables.Except(basics));
            Console.WriteLine($"Variáveis não-básicas (VNB): {notBasic} = 0");
            Console.WriteLine("Variáveis básicas (VB): ");
            foreach (var basic in _baseValues)
            {
                Console.WriteLine($"{basic.Base.Name} = {basic.Value}");
            }
            Console.WriteLine($"E a Função Objetivo: Z = {_totalValue.Value}");
        }

        #endregion
    }
}
