using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ActiveDirectory;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class ADTests : Routine
    {
        public override Title Title => "AD Tests";
        public override bool Looping => true;
        public string UserName { get; set; } = Environment.UserName;

        string[] Menu => new[]
        {
            "== change user ==",
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
                Menu
            );
            RenderListTitle(new Title(selection.SelectedItem, xtra: "[" + UserName + "]"));
            switch (selection.SelectedItem)
            {
                case "== change user ==":
                    UserName = PromptInput("User name: ");
                    break;
                case "What is my domain controller?":
                    Console.WriteLine("My domain controller: " + TextUtil.RevealNullOrBlank(ADUtil.DetectDomainController()));
                    break;
                case "What is my OU?":
                    Console.WriteLine("Lookuping OU for " + UserName + "...");
                    var info = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info.OU);
                    Console.WriteLine(info.OU.Path);
                    break;
                case "Who are the users in my OU?":
                    Console.WriteLine("Lookuping OU for " + UserName + "...");
                    var info1 = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info1.OU);
                    Console.WriteLine(info1.OU.Path);
                    Console.WriteLine();
                    Console.WriteLine("Listing users in OU...");
                    RenderList(ADUtil.ListUsersByOU(info1.OU));
                    break;
                case "Authenticate":
                    var userName = PromptInput("Enter an AD user name: ");
                    var passWord = PromptPassword("Enter the password: ");
                    var userInfo = ADUtil.Authenticate(userName, passWord);
                    if (userInfo != null)
                    {
                        Console.WriteLine("Authenticated.");
                    }
                    else
                    {
                        Console.WriteLine("Incorrect user name or password.");
                    }
                    break;
                case "Display Group Membership":
                    var listGroupMemInput = PromptInput("List group membership for " + UserName + "?  [press 'Enter' to proceed or type a different name]").Trim();
                    if (listGroupMemInput.Length > 0)
                    {
                        UserName = listGroupMemInput;
                    }
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
