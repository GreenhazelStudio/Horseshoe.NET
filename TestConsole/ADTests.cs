using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.ActiveDirectory;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Objects;

namespace TestConsole
{
    class ADTests : Routine
    {
        public override Title Title => "AD Tests";

        public override bool Looping => true;

        public string UserName { get; set; } = Environment.UserName;

        public override Title MenuTitle => new Title(base.MenuTitle, xtra: "Hello " + UserName);

        public override Action<MenuSelection<Routine>> OnMenuSelecting => (selection) => 
        {
            if (selection.SelectedItem != null)
            {
                Console.WriteLine(selection.SelectedItem.Title + " - [" + UserName + "]");
                Console.WriteLine();
            }
        };

        public override IEnumerable<Routine> Menu => new[]
        {
            new MenuItemCategoryLabel("USER ROUTINES"),
            Routine.Build
            (
                "Change user",
                () =>
                {
                    UserName = Zap.String(PromptInput("New user name (leave blank for current user): ")) ?? Environment.UserName;
                }
            ),
            Routine.Build
            (
                "Who am I?",
                () =>
                {
                    var info = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info.DisplayName + " -- " + info.EmailAddress + " -- Dept. = " + info.Department + " -- Extn 1 = " + info.ExtensionAttribute1);
                }
            ),
            Routine.Build
            (
                "What is my OU?",
                () =>
                {
                    var info = ADUtil.LookupUser(UserName);
                    Console.WriteLine("OU = " + info.OU);
                    Console.WriteLine("Path = " + info.OU.Path);
                }
            ),
            Routine.Build
            (
                "Who are the users in my OU?",
                () =>
                {
                    var info = ADUtil.LookupUser(UserName);
                    Console.WriteLine(info.OU);
                    Console.WriteLine(info.OU.Path);
                    Console.WriteLine();
                    Console.WriteLine("Listing users in OU...");
                    RenderList(ADUtil.ListUsersByOU(info.OU));
                }
            ),
            Routine.Build
            (
                "Authenticate",
                () =>
                {
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
                }
            ),
            Routine.Build
            (
                "Display Group Membership",
                () =>
                {
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
                }
            ),            
            new MenuItemCategoryLabel("DOMAIN ROUTINES"),
            Routine.Build
            (
                "What is my domain controller?",
                () =>
                {
                    var dc = ADUtil.DetectDomainController();
                    Console.WriteLine(dc.Name);
                    Console.WriteLine(dc.LdapUrl);
                }
            ),

            Routine.Build
            (
                "List OUs",
                () =>
                {
                    RenderList(ADUtil.ListOUs());
                }
            ),
            Routine.Build
            (
                "List OUs (recursively)",
                () =>
                {
                    RenderList(ADUtil.ListOUs(recursive: true));
                }
            )
        };
    }
}
