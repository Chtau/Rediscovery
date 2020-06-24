using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class FeatureHandler : BaseDisplay
    {
        private int currentNavigationIndex = 0;
        private readonly IManager _manager;

        public FeatureHandler(IManager manager)
        {
            _manager = manager;
            _manager.FeaturesCollectionChanged += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayIdentifierName()))
                {
                    WaitForWriting();
                    if (currentNavigationIndex > Collection().Count)
                        currentNavigationIndex = 0;
                    DisplayTitle();
                }
            };
        }

        private void WriteMenu()
        {

        }

        private string WriteTitle()
        {
            return "Plugin Features ";
        }

        private string DisplayIdentifierName()
        {
            return "pluginfeatures";
        }

        private IList<SharedBase.Device.FeatureDefinitionExtended> Collection()
        {
            return _manager.Features;
        }

        private bool HandleSubMenu(string[] args, string lastInput)
        {
            return false;
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            Console.Clear();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = WriteTitle(),
                Value = OnGetTitlePageIndex()
            });
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Previous.PutifyStringArray()} = Previous Feature");
            Console.WriteLine($"{Commands.Next.PutifyStringArray()} = Next Feature");
            WriteMenu();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();

            if (Collection()?.Count > 0)
            {
                var item = Collection()[currentNavigationIndex];
                Console.WriteLine();
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Name: ",
                    Value = item.DisplayName
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Author: ",
                    Value = item.Author
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Version: ",
                    Value = item.VersionText
                });
                if (!string.IsNullOrWhiteSpace(item.Website))
                {
                    ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                    {
                        Color = ConsoleColor.White,
                        Prefix = "Website: ",
                        Value = item.Website
                    });
                }
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Control integration: ",
                    Value = Enum.GetName(typeof(SharedBase.Device.IntegrationPoint), item.ControlIntegrationPoint)
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Feature integration: ",
                    Value = Enum.GetName(typeof(SharedBase.Device.IntegrationPoint), item.FeatureIntegrationPoint)
                });


                Console.WriteLine();
                Console.WriteLine();
            }

            Console.Write("Command:");
            isWriting = false;
        }

        private string OnGetTitlePageIndex()
        {
            string retVal = "";
            if (Collection() == null || Collection().Count == 0)
            {
                retVal = " no features found";
            }
            else
            {
                retVal = (currentNavigationIndex + 1).ToString() + " / " + Collection()?.Count.ToString();
            }

            return retVal;
        }

        public override void Handle(string[] args)
        {
            SharedUI.CurrentDisplay = DisplayIdentifierName();
            DisplayTitle();
            string lastInput = null;
            do
            {
                if (Collection()?.Count > 0 && Commands.MatchInput(lastInput, Commands.Previous))
                {
                    lastInput = null;
                    if (currentNavigationIndex == 0)
                        currentNavigationIndex = Collection().Count - 1;
                    else
                        currentNavigationIndex--;
                }
                else if (Collection()?.Count > 0 && Commands.MatchInput(lastInput, Commands.Next))
                {
                    lastInput = null;
                    currentNavigationIndex++;
                    if (currentNavigationIndex >= Collection().Count)
                        currentNavigationIndex = 0;
                }
                else
                {
                    if (HandleSubMenu(args, lastInput))
                        lastInput = null;
                    else
                        lastInput = Console.ReadKey().KeyChar.ToString();
                }
            } while (ResetOrBack(lastInput));
        }
    }
}
