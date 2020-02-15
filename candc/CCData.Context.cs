﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CC
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AntigenAudit> AntigenAudits { get; set; }
        public virtual DbSet<Antigen> Antigens { get; set; }
        public virtual DbSet<ArrayAntigen> ArrayAntigens { get; set; }
        public virtual DbSet<Array> Arrays { get; set; }
        public virtual DbSet<CalibControl> CalibControls { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
    
        public virtual ObjectResult<GetAntigensNotAssingedToBatch_Result> GetAntigensNotAssingedToBatch()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAntigensNotAssingedToBatch_Result>("GetAntigensNotAssingedToBatch");
        }
    
        public virtual ObjectResult<GetArrayAntigenRelations_Result> GetArrayAntigenRelations()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetArrayAntigenRelations_Result>("GetArrayAntigenRelations");
        }
    
        public virtual ObjectResult<AntigensAssingedToArray> GetAntigensAssingedToArray()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AntigensAssingedToArray>("GetAntigensAssingedToArray");
        }
    
        public virtual ObjectResult<GetAntigensNotAssingedToArray_Result> GetAntigensNotAssingedToArray()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAntigensNotAssingedToArray_Result>("GetAntigensNotAssingedToArray");
        }
    }
}
