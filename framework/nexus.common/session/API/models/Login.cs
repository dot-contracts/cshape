using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nexus.common
{
    public class LoginRequest
    {
        [Required]    public string  Username   { get; set; } = string.Empty;
        [Required]    public string  Password   { get; set; } = string.Empty;
                      public string? LocationId { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Status   { get; set; } = String.Empty;
        public string Response { get; set; } = String.Empty;
        public string Token    { get; set; } = String.Empty;

        public string   WorkerType     { get; set; } = string.Empty;
        public string   WorkerState    { get; set; } = string.Empty;
        public string   Department     { get; set; } = string.Empty;
        public string   Gender         { get; set; } = string.Empty;
        public string   Title          { get; set; } = string.Empty;
        public string   FirstName      { get; set; } = string.Empty;
        public string   OtherName      { get; set; } = string.Empty;
        public string   LastName       { get; set; } = string.Empty;
        public string   BirthDate      { get; set; } = string.Empty;
        public string   EMail          { get; set; } = string.Empty;
        public string   Phone          { get; set; } = string.Empty;
        public bool     IsApproved     { get; set; } = false ;
        public string   LastUse        { get; set; } = string.Empty;
        public string   PasswordHash   { get; set; } = string.Empty;
        public string   DefaultMenu    { get; set; } = string.Empty;
        public string   DefaultMode    { get; set; } = string.Empty;
        public UserRoles Role          { get; set; } = UserRoles.Guest;
        public int      WorkerId       { get; set; } = -1;
        public int      WorkerTypeId   { get; set; } = -1;
        public int      WorkerStateId  { get; set; } = -1;
        public int      GenderId       { get; set; } = -1;
        public int      TitleId        { get; set; } = -1;
        public Menu[]   Menus          { get; set; } = Array.Empty<Menu>();
    }

    public enum UserRoles
    {
        Guest,              // Basic public access, no backend privileges
        Housekeeping,       // Limited access to room areas only
        FoodAndBeverage,    // Access to POS systems, limited guest data
        Attendant,          // Frontline EGM interaction, limited system access
        Dealer,             // Handles chips and cash at tables, no system access
        Concierge,          // Guest service tools, limited booking access
        FrontDesk,          // Reservation and billing systems, some personal data
        Maintenance,        // Access to physical systems and infrastructure
        Marketing,          // Guest contact info, promotions, loyalty system
        Security,           // Physical security and incident systems
        PitBoss,            // Oversees gaming floors, staff scheduling
        Cashier,            // Handles money and chips, moderate system access
        Gaming,             // Access to gaming data, player tracking, revenue
        Surveillance,       // High sensitivity — video systems, incident logs
        ITSupport,          // Backend systems, credentials, network access
        Manager,            // Department-wide control, elevated privileges
        Auditor,            // Regulatory logs, financial and system audit access
        Admin               // Full control, user management, system settings
    }
}



