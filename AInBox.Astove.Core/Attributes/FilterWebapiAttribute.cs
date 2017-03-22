using System;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterWebapiAttribute : Attribute
    {
        private int groupOrder = int.MaxValue;

        /// <summary>
        /// Posição do grupo de filtros
        /// </summary>
        public int GroupOrder
        {
            get
            {
                return groupOrder;
            }
            set
            {
                groupOrder = value;
            }
        }

        /// <summary>
        /// Posição do filtro no grupo
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Permissão necessária para apresentar o filtro
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Nome que será apresentado no label do filtro
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Tipo do filtro
        /// </summary>
        public Type FilterType { get; set; }

        /// <summary>
        /// Index para controle de multiplo atributos
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Valor padrão do filtro
        /// Ex.: [today]
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Valor padrão do operador do filtro
        /// Ex.: Igual, Menor igual, Maior igual
        /// </summary>
        private int defaultOperator = -1;

        public int DefaultOperator 
        {
            get { return defaultOperator; }
            set { defaultOperator = value; }
        }

        /// <summary>
        /// Nome da coluna para aplicação do filtro, por padrão é passado o nome da propriedade
        /// Obs.: Não utilizar o prefixo 'it.'
        /// Exemplo: Descricao
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Nome da coluna do objeto da coleção que será aplicado o fitro exists.
        /// Caso seja uma outra entidade pai, utilizar entidade.Property
        /// Obs.: Não utilizar o prefixo 'it.'
        /// Exemplo: NomeFantasia, ou empresa.NomeFantasia
        /// </summary>
        public string FilterExistsColumn { get; set; }

        /// <summary>
        /// Css do filtro, ex: datepicker ou date
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Máscara do filtro, ex: 99.999.999-99
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// Largura em pixels do filtro, ex: 100
        /// </summary>
        private int width = -1;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Comprimento máximo de uma cadeia de caracteres, ex: 100
        /// </summary>
        private int length = -1;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Condição do filtro no banco de dados
        /// </summary>
        public string Where { get; set; }

        public string OrderBy { get; set; }

        public FilterWebapiAttribute() { this.FilterType = this.GetType(); }
        public FilterWebapiAttribute(Type filterType) { this.FilterType = filterType; }

        public void SetIndex(int index)
        {
            this.Index = index;
        }

        public bool Internal { get; set; }
        public Type InternalType { get; set; }
        public string PreCondition { get; set; }
        public string PosCondition { get; set; }

        public override object TypeId
        {
            get
            {
                return string.Format("{0}-{1}", this.Property, this.FilterType.Name);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterLikeAttribute : FilterWebapiAttribute
    {
        public FilterLikeAttribute() : base(typeof(LikeFilter)) { }
        
        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterDateAttribute : FilterWebapiAttribute
    {
        public FilterDateAttribute() : base(typeof(DateFilter)) { }

        /// <summary>
        /// Adiciona ou remove dias no valor default do filtro (utilizar em conjunto com [today] no DefaultValue. 
        /// Ex: DefaultValueAddDays(1) para amanhã, DefaultValueAddDays(-1) para ontem
        /// </summary>
        public int DefaultValueAddDays { get; set; }

        /// <summary>
        /// Habilita ou desabilita a inclusão de hora inicial (00:00:00) no filtro
        /// </summary>
        public bool AppendFirstTime { get; set; }

        /// <summary>
        /// Habilita ou desabilita a inclusão de hora final (23:59:59) no filtro
        /// </summary>        
        public bool AppendLastTime { get; set; }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterBirthdayAttribute : FilterWebapiAttribute
    {
        public FilterBirthdayAttribute() : base(typeof(BirthdayFilter)) { }

        /// <summary>
        /// Habilita ou desabilita a inclusão de hora inicial (00:00:00) no filtro
        /// </summary>
        public bool AppendFirstTime { get; set; }

        /// <summary>
        /// Habilita ou desabilita a inclusão de hora final (23:59:59) no filtro
        /// </summary>        
        public bool AppendLastTime { get; set; }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterDecimalAttribute : FilterWebapiAttribute
    {
        public FilterDecimalAttribute() : base(typeof(DecimalFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterAgeAttribute : FilterWebapiAttribute
    {
        public FilterAgeAttribute() : base(typeof(AgeFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterFKDropDownListAttribute : FilterWebapiAttribute
    {
        /// <summary>
        /// Habilitar pesquisa na caixa de seleção
        /// </summary>
        public bool EnableSearch { get; set; }

        /// <summary>
        /// Metodo deverá retornar o valor default para o fitro
        /// </summary>        
        public string StaticDefaultMethod { get; set; }

        /// <summary>
        /// Metodo deverá retornar a lista de objetos para popular a combobox.
        /// </summary>
        public string StaticPopulateMethod { get; set; }

        /// <summary>
        /// Nome do type que contém o método estático.
        /// </summary>
        public string ManagerType { get; set; }

        /// <summary>
        /// Nome da propriedade para apresentação do texto.
        /// </summary>
        public string TextField { get; set; }

        private string valueField = "Id";

        /// <summary>
        /// Nome da propriedade com valor único.
        /// </summary>
        public string ValueField { get { return valueField; } set { valueField = value; } }

        /// <summary>
        /// Informe para carregar os itens da lista com objetos de um tipo do Model diferente do tipo da coluna.
        /// Ex.: category
        /// </summary>
        public Type EntityType { get; set; }

        public string EntityName { get; set; }

        public FilterFKDropDownListAttribute() : base(typeof(FKDropdownListFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterFKCheckBoxAttribute : FilterWebapiAttribute
    {
        /// <summary>
        /// Metodo deverá recitornar o valor default para o fitro
        /// </summary>        
        public string StaticDefaultMethod { get; set; }

        /// <summary>
        /// Metodo deverá recitornar a lista de objetos para popular a combobox.
        /// </summary>
        public string StaticPopulateMethod { get; set; }

        /// <summary>
        /// Nome do type que contém o método estático.
        /// </summary>
        public string ManagerType { get; set; }

        /// <summary>
        /// Nome da propriedade para apresentação do texto.
        /// </summary>
        public string TextField { get; set; }

        private string valueField = "Id";

        /// <summary>
        /// Nome da propriedade com valor único.
        /// </summary>
        public string ValueField { get { return valueField; } set { valueField = value; } }

        public FilterFKCheckBoxAttribute() : base(typeof(FKCheckBoxFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterEnumAttribute : FilterWebapiAttribute
    {
        ///// <summary>
        ///// Nome da propriedade para apresentação do texto.
        ///// </summary>
        //public string TextField { get; set; }

        //private string valueField = "Value";

        ///// <summary>
        ///// Nome da propriedade com valor único.
        ///// </summary>
        //public string ValueField { get { return valueField; } set { valueField = value; } }

        /// <summary>
        /// Tipo do Enumerador. Ex: typeof(Triade.Arquitetura.Objetos.Enums.Tipo)"
        /// </summary>
        public Type EnumType { get; set; }

        public FilterEnumAttribute() : base(typeof(EnumFilter)) { }

        public override object TypeId
        {
            get
            {
                return System.Guid.NewGuid();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterBooleanAttribute : FilterWebapiAttribute
    {
        public FilterBooleanAttribute() : base(typeof(BooleanFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterDecimalCompareAttribute : FilterWebapiAttribute
    {
        public string FilterColumnToCompare { get; set; }

        public FilterDecimalCompareAttribute() : base(typeof(DecimalCompareFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterExistsAttribute : FilterWebapiAttribute
    {
        public FilterExistsAttribute() : base(typeof(ExistsFilter)) { }

        //public override object TypeId
        //{
        //    get
        //    {
        //        return System.Guid.NewGuid();
        //    }
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterAssyncAutoCompleteAttribute : FilterWebapiAttribute
    {
        /// <summary>
        /// Colunas que serão pesquisadas ex.: new string[] { "Nome", "Endereco" }
        /// </summary>
        public string[] ColumnsToSearch { get; set; }

        /// <summary>
        /// Limite de registros da busca. Default = 10
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Quantidade minima de caracteres para executar a pesquisa, Default = 0.
        /// </summary>
        public int MinChars { get; set; }

        /// <summary>
        /// Metodo statico da classe da chave estrangeira para obter a lista de itens
        /// Child -> Parent (FK), o metodo deverá ser implementado na classe parent
        /// Ex.: public static List<Parent> GetParents() 
        /// </summary>
        public string PopulateMethod { get; set; }

        /// <summary>
        /// Informe o filtro para filtrar os itens.
        /// Ex.: it.Ativo = true
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Nome do método estático para capturar a alteração no controle
        /// Ex.: public static void Field_Changed(Triade.Arquitetura.Web.DataDynamic.DynamicFieldTemplate sender, int? value)
        /// </summary>
        public string ChangedStaticMethod { get; set; }

        /// <summary>
        /// Nome do método estático para definir o filtro em tempo de execução
        /// Ex.: public static string Filter(string defaultFilter)
        /// </summary>        
        public string FilterStaticMethod { get; set; }

        /// <summary>
        /// Informe para carregar os itens da lista com objetos de um tipo do Model diferente do tipo da coluna.
        /// Ex: typeof(AInBox.Dynamic.produto)
        /// </summary>
        public System.Type EntityType { get; set; }

        public FilterAssyncAutoCompleteAttribute() : base(typeof(AssyncAutoCompleteFilter)) { }
    }
}