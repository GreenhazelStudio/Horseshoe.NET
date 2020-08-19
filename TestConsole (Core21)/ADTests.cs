using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ActiveDirectory;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Objects.Clean;

namespace TestConsole
{
    class ADTests : Routine
    {
        public override Title Title => "AD Tests";
        public override bool Looping => true;
        public string UserName { get; set; } = Environment.UserName;

        string[] Menu => new[]
        {
            "Change user",
            "Who am I?",
            "What is my domain controller?",
            "What is my OU?",
            "Who are the users in my OU?",
            "Authenticate",
            "Display Group Membership",
            "List OUs",
            "List OUs (recursively)",
            "List Users",
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu,
                title: new Title("AD Tests", xtra: "Hello " + UserName)
            );
            UserInfo info;
            RenderListTitle(new Title(selection.SelectedItem, xtra: "[" + UserName + "]"));
            switch (selection.SelectedItem)
            {
                case "Change user":
                    UserName = Zap.String(PromptInput("New user name (leave blank for current user): ")) ?? Environment.UserName;
                    break;
                case "Who am I?":
                    info = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info.DisplayName + " -- " + info.EmailAddress + " -- Dept. = " + info.Department + " -- Extn 1 = " + info.ExtensionAttribute1);
                    break;
                case "What is my domain controller?":
                    var dc = ADUtil.DetectDomainController();
                    Console.WriteLine(dc.Name);
                    Console.WriteLine(dc.LdapUrl);
                    break;
                case "What is my OU?":
                    info = ADUtil.LookupUser(UserName);
                    Console.WriteLine("OU = " + info.OU);
                    Console.WriteLine("Path = " + info.OU.Path);
                    break;
                case "Who are the users in my OU?":
                    info = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info.OU);
                    Console.WriteLine(info.OU.Path);
                    Console.WriteLine();
                    Console.WriteLine("Listing users in OU...");
                    RenderList(ADUtil.ListUsersByOU(info.OU));
                    break;
                case "Authenticate":
                    var passWord = PromptPassword("Enter the password for " + UserName + ": ");
                    UserInfo userInfo;
                    try
                    {
                        userInfo = ADUtil.Authenticate(UserName, passWord);
                        if (userInfo != null)
                        {
                            Console.WriteLine("Authenticated.");
                        }
                        else
                        {
                            Console.WriteLine("Incorrect user name or password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        RenderException(ex);
                    }
                    break;
                case "Display Group Membership":
                    var user = ADUtil.LookupUser(UserName);
                    if (user != null)
                    {
                        Console.WriteLine("Listing group membership for " + UserName + "...");
                        RenderList(user.GroupNames, title: "Group Membership");
                    }
                    else
                    {
                        RenderAlert("User not found");
                    }
                    break;
                case "List OUs":
                    RenderList(ADUtil.ListOUs());
                    break;
                case "List OUs (recursively)":
                    RenderList(ADUtil.ListOUs(recursive: true));
                    break;
            }
        }
    }
}
