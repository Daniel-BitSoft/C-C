//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDisabled { get; set; }
        public bool RequirePasswordChange { get; set; }
        public System.DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public int LockCounter { get; set; }
    }
}
