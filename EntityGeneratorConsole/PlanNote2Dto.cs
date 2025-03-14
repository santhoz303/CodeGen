using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileToEntityGenerator.Models
{
    public class PlanNote2Dto
    {
        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string OrdinalPosition { get; set; }

        public string ColumnDefault { get; set; }

        public string IsNullable { get; set; }

        public string DataType { get; set; }

        public string CharacterMaximumLength { get; set; }

        public string CharacterOctetLength { get; set; }

        public string NumericPrecision { get; set; }

        public string NumericPrecisionRadix { get; set; }

        public string NumericScale { get; set; }

        public string DatetimePrecision { get; set; }

        public string CharacterSetCatalog { get; set; }

        public string CharacterSetSchema { get; set; }

        public string CharacterSetName { get; set; }

        public string CollationCatalog { get; set; }

        public string CollationSchema { get; set; }

        public string CollationName { get; set; }

        public string DomainCatalog { get; set; }

        public string DomainSchema { get; set; }

        public string DomainName { get; set; }

    }
}
