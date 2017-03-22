using System;
using System.ComponentModel;

namespace AInBox.Astove.Core.Options
{
    public enum BooleanOperator
    {
        [Description("[igual]")]
        Igual = 0,
        [Description("[diferente]")]
        Diferente = 1,
        [Description("[existe]")]
        Existe = 2,
        [Description("[não existe]")]
        NaoExiste = 3
    }

    public enum StringOperator
    {
        [Description("[contém]")]
        Contem = 0,
        [Description("[igual]")]
        Igual = 1,
        [Description("[diferente]")]
        Diferente = 2,
        [Description("[começa com]")]
        ComecaCom = 3,
        [Description("[termina com]")]
        TerminaCom = 4,
        [Description("[existe]")]
        Existe = 5,
        [Description("[não existe]")]
        NaoExiste = 6
    }

    public enum ValueOperator
    {
        [Description("[igual]")]
        Igual = 0,
        [Description("[diferente]")]
        Diferente = 1,
        [Description("[maior]")]
        Maior = 2,
        [Description("[maior ou igual]")]
        MaiorIgual = 3,
        [Description("[menor]")]
        Menor = 4,
        [Description("[menor igual]")]
        MenorIgual = 5,
        [Description("[existe]")]
        Existe = 6,
        [Description("[não existe]")]
        NaoExiste = 7
    }

    public enum DateOperator
    {
        [Description("[igual]")]
        Igual = 0,
        [Description("[maior]")]
        Maior = 1,
        [Description("[maior ou igual]")]
        MaiorIgual = 2,
        [Description("[menor]")]
        Menor = 3,
        [Description("[menor igual]")]
        MenorIgual = 4,
        [Description("[existe]")]
        Existe = 5,
        [Description("[não existe]")]
        NaoExiste = 6
    }
}

namespace AInBox.Astove.Core.Options.Internal
{
    public enum BooleanOperator
    {
        [Description("{0} == @{1}")]
        Igual = 0,
        [Description("{0} != @{1}")]
        Diferente = 1,
        [Description("{0} != null")]
        Existe = 2,
        [Description("{0} == null")]
        NaoExiste = 3
    }

    public enum StringOperator
    {
        [Description("{0}.ToLower().Contains(@{1}.ToLower())")]
        Contem = 0,
        [Description("{0}.ToLower() == @{1}.ToLower()")]
        Igual = 1,
        [Description("{0}.ToLower() != @{1}.ToLower()")]
        Diferente = 2,
        [Description("{0}.ToLower().StartsWith(@{1}.ToLower())")]
        ComecaCom = 3,
        [Description("{0}.ToLower().EndsWith(@{1}.ToLower())")]
        TerminaCom = 4,
        [Description("{0} != null")]
        Existe = 5,
        [Description("{0} == null")]
        NaoExiste = 6
    }

    public enum ValueOperator
    {
        [Description("{0} == @{1}")]
        Igual = 0,
        [Description("{0} != @{1}")]
        Diferente = 1,
        [Description("{0} > @{1}")]
        Maior = 2,
        [Description("{0} >= @{1}")]
        MaiorIgual = 3,
        [Description("{0} < @{1}")]
        Menor = 4,
        [Description("{0} <= @{1}")]
        MenorIgual = 5,
        [Description("{0} != null")]
        Existe = 6,
        [Description("{0} == null")]
        NaoExiste = 7
    }

    public enum DateOperator
    {
        [Description("{0} == @{1}")]
        Igual = 0,
        [Description("{0} > @{1}")]
        Maior = 1,
        [Description("{0} >= @{1}")]
        MaiorIgual = 2,
        [Description("{0} < @{1}")]
        Menor = 3,
        [Description("{0} <= @{1}")]
        MenorIgual = 4,
        [Description("{0} != null")]
        Existe = 5,
        [Description("{0} == null")]
        NaoExiste = 6
    }
}

namespace AInBox.Astove.Core.Options.Internal.Full
{
    public enum BooleanOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 0,
        [Description("{0} {1} != @{2} {3}")]
        Diferente = 1,
        [Description("{0} {1} != null {3}")]
        Existe = 2,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 3
    }

    public enum StringOperator
    {
        [Description("{0} {1}.ToLower().Contains(@{2}.ToLower()) {3}")]
        Contem = 0,
        [Description("{0} {1}.ToLower() == @{2}.ToLower() {3}")]
        Igual = 1,
        [Description("{0} {1}.ToLower() != @{2}.ToLower() {3}")]
        Diferente = 2,
        [Description("{0} {1}.ToLower().StartsWith(@{2}.ToLower()) {3}")]
        ComecaCom = 3,
        [Description("{0} {1}.ToLower().EndsWith(@{2}.ToLower()) {3}")]
        TerminaCom = 4,
        [Description("{0} {1} != null {3}")]
        Existe = 5,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 6
    }

    public enum ValueOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 0,
        [Description("{0} {1} != @{2} {3}")]
        Diferente = 1,
        [Description("{0} {1} > @{2} {3}")]
        Maior = 2,
        [Description("{0} {1} >= @{2} {3}")]
        MaiorIgual = 3,
        [Description("{0} {1} < @{2} {3}")]
        Menor = 4,
        [Description("{0} {1} <= @{2} {3}")]
        MenorIgual = 5,
        [Description("{0} {1} != null {3}")]
        Existe = 6,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 7
    }

    public enum DateOperator
    {
        [Description("{0} {1} == @{2} {3}")]
        Igual = 0,
        [Description("{0} {1} > @{2} {3}")]
        Maior = 1,
        [Description("{0} {1} >= @{2} {3}")]
        MaiorIgual = 2,
        [Description("{0} {1} < @{2} {3}")]
        Menor = 3,
        [Description("{0} {1} <= @{2} {3}")]
        MenorIgual = 4,
        [Description("{0} {1} != null {3}")]
        Existe = 5,
        [Description("{0} {1} == null {3}")]
        NaoExiste = 6
    }
}