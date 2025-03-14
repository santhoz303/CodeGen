using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileToEntityGenerator.Models
{
    public class PlanNote2Entity
    {
        [Column("TABLE_CATALOG")]
        public string TableCatalog { get; set; }

        [Column("TABLE_SCHEMA")]
        public string TableSchema { get; set; }

        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }

        [Column("ORDINAL_POSITION")]
        public string OrdinalPosition { get; set; }

        [Column("COLUMN_DEFAULT")]
        public string ColumnDefault { get; set; }

        [Column("IS_NULLABLE")]
        public string IsNullable { get; set; }

        [Column("DATA_TYPE")]
        public string DataType { get; set; }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public string CharacterMaximumLength { get; set; }

        [Column("CHARACTER_OCTET_LENGTH")]
        public string CharacterOctetLength { get; set; }

        [Column("NUMERIC_PRECISION")]
        public string NumericPrecision { get; set; }

        [Column("NUMERIC_PRECISION_RADIX")]
        public string NumericPrecisionRadix { get; set; }

        [Column("NUMERIC_SCALE")]
        public string NumericScale { get; set; }

        [Column("DATETIME_PRECISION")]
        public string DatetimePrecision { get; set; }

        [Column("CHARACTER_SET_CATALOG")]
        public string CharacterSetCatalog { get; set; }

        [Column("CHARACTER_SET_SCHEMA")]
        public string CharacterSetSchema { get; set; }

        [Column("CHARACTER_SET_NAME")]
        public string CharacterSetName { get; set; }

        [Column("COLLATION_CATALOG")]
        public string CollationCatalog { get; set; }

        [Column("COLLATION_SCHEMA")]
        public string CollationSchema { get; set; }

        [Column("COLLATION_NAME")]
        public string CollationName { get; set; }

        [Column("DOMAIN_CATALOG")]
        public string DomainCatalog { get; set; }

        [Column("DOMAIN_SCHEMA")]
        public string DomainSchema { get; set; }

        [Column("DOMAIN_NAME")]
        public string DomainName { get; set; }

    }
}
