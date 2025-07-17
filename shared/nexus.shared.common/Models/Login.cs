using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nexus.shared.common;

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
    public UserRoles Role { get; set; } = UserRoles.Admin;// .Student;
    public int      WorkerId       { get; set; } = -1;
    public int      WorkerTypeId   { get; set; } = -1;
    public int      WorkerStateId  { get; set; } = -1;
    public int      GenderId       { get; set; } = -1;
    public int      TitleId        { get; set; } = -1;
    public Menu[]   Menus          { get; set; } = Array.Empty<Menu>();
}



