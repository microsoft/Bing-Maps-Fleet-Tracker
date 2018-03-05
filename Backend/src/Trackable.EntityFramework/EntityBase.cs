// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackable.EntityFramework
{
    public class EntityBase<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        public bool Deleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAtTimeUtc { get; set; }
    }

    public class EntityBase<TKey1, TKey2>
    {
        [Key, Column(Order = 0)]
        public TKey1 Key1 { get; set; }

        [Key, Column(Order = 1)]
        public TKey2 Key2 { get; set; }

        public bool Deleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAtTimeUtc { get; set; }
    }
}
