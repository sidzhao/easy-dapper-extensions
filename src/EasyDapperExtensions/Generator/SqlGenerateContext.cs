using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using EasyDapperExtensions.Generator.Adapter;

namespace EasyDapperExtensions.Generator
{
    internal class SqlGenerateContext
    {
        public string TableName { get; set; }

        public ISqlAdapter Adapter { get; set; }

        public PropertyInfo[] AllProperties { get; set; }

        public List<SqlPropertyMetadata> SqlProperties { get; set; }


        public SqlPropertyMetadata[] KeySqlProperties
        {
            get
            {
                if (_keySqlProperties == null)
                {
                    if (SqlProperties == null)
                    {
                        _keySqlProperties = new SqlPropertyMetadata[0];
                    }
                    else
                    {
                        _keySqlProperties = SqlProperties.Where(p => p.IsKey).ToArray();

                        // If not found specified key property with KeyAttribute, then try to find the property named "id". Set it as primary key if find.
                        if (!_keySqlProperties.Any())
                        {
                            _keySqlProperties = SqlProperties.Where(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase)).ToArray();

                            foreach (var keySqlProperty in _keySqlProperties)
                            {
                                keySqlProperty.IsKey = true;
                            }
                        }
                    }
                }

                return _keySqlProperties;
            }
        }
        private SqlPropertyMetadata[] _keySqlProperties;

        public SqlPropertyMetadata[] NonKeySqlProperties
        {
            get
            {
                if (_nonKeySqlProperties == null)
                {
                    if (SqlProperties == null || KeySqlProperties == null)
                    {
                        _nonKeySqlProperties = new SqlPropertyMetadata[0];
                    }
                    else
                    {
                        _nonKeySqlProperties = SqlProperties.Where(p => !KeySqlProperties.Contains(p)).ToArray();
                    }
                }

                return _nonKeySqlProperties;
            }
        }

        private SqlPropertyMetadata[] _nonKeySqlProperties;

        public SqlPropertyMetadata[] IdentitySqlProperties
        {
            get
            {
                if (_identitySqlProperties == null)
                {
                    if (SqlProperties == null)
                    {
                        _identitySqlProperties = new SqlPropertyMetadata[0];
                    }
                    else
                    {
                        _identitySqlProperties = SqlProperties?.Where(p => p.IsIdentity).ToArray();

                        // If not found specified key property with KeyAttribute, then try to find the property named "id". Set it as primary key if find.
                        if (!_identitySqlProperties.Any())
                        {
                            _identitySqlProperties = SqlProperties.Where(p => p.SpecifiedDatabaseGeneratedOption != DatabaseGeneratedOption.None && string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase)).ToArray();

                            foreach (var identitySqlProperty in _identitySqlProperties)
                            {
                                identitySqlProperty.IsIdentity = true;
                            }
                        }
                    }
                }

                return _identitySqlProperties;
            }
        }
        private SqlPropertyMetadata[] _identitySqlProperties;

        public SqlPropertyMetadata[] NonIdentitySqlProperties
        {
            get
            {
                if (_nonIdentitySqlProperties == null)
                {
                    if (SqlProperties == null || IdentitySqlProperties == null)
                    {
                        _nonKeySqlProperties = new SqlPropertyMetadata[0];
                    }
                    else
                    {
                        _nonIdentitySqlProperties = SqlProperties.Where(p => !IdentitySqlProperties.Contains(p)).ToArray();
                    }
                }

                return _nonIdentitySqlProperties;
            }
        }
        private SqlPropertyMetadata[] _nonIdentitySqlProperties;

        public bool HasIdentitySqlProperty => IdentitySqlProperties.Any();
    }
}
