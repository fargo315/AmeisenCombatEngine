using AmeisenCombatEngineCore;
using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.FSM.Enums;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Strategies;
using AmeisenCombatEngineCore.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AmeisenCombatEngine.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Unit Me { get; set; }
        private Unit Target { get; set; }

        private CombatEngine CombatEngine { get; set; }
        private CombatEngine CombatEngine2 { get; set; }

        private List<Spell> Spells { get; set; }
        private Spell ActiveSpellForEnemy { get; set; }

        private bool FightIsOver { get; set; }
        private int ScoreMe { get; set; }
        private int ScoreTarget { get; set; }

        public MainWindow()
        {
            // Some sample spells
            Spells = new List<Spell>()
            {
                new Spell
                (
                    "Hit",
                    0,
                    3,
                    9,
                    SpellType.Damage,
                    SpellExecution.Melee,
                    new Dictionary<SpellType, double>()
                    {
                        { SpellType.Damage, 100}
                    }
                ),
                new Spell
                (
                    "Big Hit",
                    0,
                    3,
                    9,
                    SpellType.Damage,
                    SpellExecution.Melee,
                    new Dictionary<SpellType, double>()
                    {
                        { SpellType.Damage, 400}
                    }
                ),
                new Spell
                (
                    "Heal",
                    220,
                    30,
                    9,
                    SpellType.Heal,
                    SpellExecution.Cast,
                    new Dictionary<SpellType, double>()
                    {
                        { SpellType.Heal, 300}
                    }
                ),
                new Spell
                (
                    "Big Heal",
                    360,
                    30,
                    9,
                    SpellType.Heal,
                    SpellExecution.Cast,
                    new Dictionary<SpellType, double>()
                    {
                        { SpellType.Heal, 800}
                    }
                )
            };

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Spell spell in Spells)
            {
                spellSelection.Items.Add(spell);
            }
            spellSelection.SelectedIndex = 0;

            InitCombatEngine();
        }

        private void InitCombatEngine()
        {
            FightIsOver = false;
            combatlogMe.Document.Blocks.Clear();
            combatlogTarget.Document.Blocks.Clear();

            /*
            Random rnd = new Random();

            int health = rnd.Next(20000, 30000);
            int healthTarget = rnd.Next(20000, 30000);

            int energy = rnd.Next(5000, 10000);
            int energyTarget = rnd.Next(5000, 10000);
            */

            int health = 20000;
            int healthTarget = 20000;

            int energy = 10000;
            int energyTarget = 10000;

            Vector3 positionMe = new Vector3(0, 0, 0);
            Vector3 positionTarget = new Vector3(0, 0, 0);

            Me = new Unit(health, health, energy, energy, CombatState.Standing, positionMe);
            Target = new Unit(healthTarget, healthTarget, energyTarget, energyTarget, CombatState.Standing, positionTarget);

            this.Dispatcher.Invoke(UpdateViews);

            CombatEngine = new CombatEngine(Me, Target, Spells, new DpsSimpleStrategy(Spells, 30));
            CombatEngine2 = new CombatEngine(Target, Me, Spells, new DpsSimpleStrategy(Spells, 70));

            LogCombatActionMe("Initializing...");
            LogCombatActionTarget("Initializing...");
        }

        private void ButtonDoIteration_Click(object sender, RoutedEventArgs e)
        {
            DoIteration();
        }

        private void DoIteration(bool updateViews = true)
        {
            if (FightIsOver)
            {
                return;
            }

            if (Me.Health <= 0)
            {
                if (updateViews)
                {
                    this.Dispatcher.Invoke(() => LogCombatActionMe($"I lost the fight..."));
                    this.Dispatcher.Invoke(() => LogCombatActionTarget($"I won the fight..."));
                }

                ScoreTarget++;
                FightIsOver = true;

                if (updateViews)
                {
                    this.Dispatcher.Invoke(UpdateViews);
                }
                return;
            }

            if (Target.Health <= 0)
            {
                if (updateViews)
                {
                    this.Dispatcher.Invoke(() => LogCombatActionMe($"I won the fight..."));
                    this.Dispatcher.Invoke(() => LogCombatActionTarget($"I lost the fight..."));
                }

                ScoreMe++;
                FightIsOver = true;

                if (updateViews)
                {
                    this.Dispatcher.Invoke(UpdateViews);
                }
                return;
            }

            if (Me.Energy + 5 <= Me.MaxEnergy)
            {
                Me.Energy += 5;
            }

            if (Target.Energy + 5 <= Target.MaxEnergy)
            {
                Target.Energy += 5;
            }

            string usedSpellName = CombatEngine.DoIteration();
            string usedSpellName2 = CombatEngine2.DoIteration();

            if (updateViews)
            {
                this.Dispatcher.Invoke(() => LogCombatActionMe($"Use Spell: {usedSpellName}"));
            }

            if (usedSpellName != "")
            {
                Spell usedSpell = Spells
                .Where(spell => spell.SpellName == usedSpellName)
                .First();

                ProcessSpellUsage(usedSpell, Me, Target, updateViews);
            }

            if (usedSpellName2 != "")
            {
                Spell usedSpell2 = Spells
                .Where(spell => spell.SpellName == usedSpellName2)
                .First();

                ProcessSpellUsage(usedSpell2, Target, Me, updateViews);
            }
        }

        private void ProcessSpellUsage(Spell usedSpell, Unit a, Unit b, bool updateViews = true)
        {
            usedSpell.StartCooldown();
            if (usedSpell.EnergyCost > a.Energy)
            {
                if (updateViews)
                {
                    if (a == Me)
                    {
                        this.Dispatcher.Invoke(() => LogCombatActionMe($"Out of energy...", "#4286ff"));
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() => LogCombatActionTarget($"Out of energy...", "#4286ff"));
                    }
                }
                return;
            }

            a.Energy -= usedSpell.EnergyCost;
            foreach (KeyValuePair<SpellType, double> spellImpact in usedSpell.SpellImpacts)
            {
                switch (spellImpact.Key)
                {
                    case SpellType.Damage:
                        b.Health -= spellImpact.Value;

                        if (updateViews)
                        {
                            if (a == Me)
                            {
                                this.Dispatcher.Invoke(() => LogCombatActionMe($"Damage done: {spellImpact.Value}", "#ff4e42"));
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() => LogCombatActionTarget($"Damage done: {spellImpact.Value}", "#ff4e42"));
                            }
                        }

                        this.Dispatcher.Invoke(UpdateViews);
                        break;

                    case SpellType.Heal:
                        a.Health += spellImpact.Value;

                        if (a.Health > a.MaxHealth)
                        {
                            a.Health = a.MaxHealth;
                        }

                        if (updateViews)
                        {
                            if (a == Me)
                            {
                                this.Dispatcher.Invoke(() => LogCombatActionMe($"Healing done: {spellImpact.Value}", "#77f442"));
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() => LogCombatActionTarget($"Healing done: {spellImpact.Value}", "#77f442"));
                            }
                        }

                        this.Dispatcher.Invoke(UpdateViews);
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateViews()
        {
            healthMe.Value = Me.Health;
            healthTarget.Value = Target.Health;

            healthMe.Maximum = Me.MaxHealth;
            healthTarget.Maximum = Target.MaxHealth;

            healthlabelMe.Content = $"{Me.Health} / {Me.MaxHealth}";
            healthlabelTarget.Content = $"{Target.Health} / {Target.MaxHealth}";

            energyMe.Value = Me.Energy;
            energyTarget.Value = Target.Energy;

            energyMe.Maximum = Me.MaxEnergy;
            energyTarget.Maximum = Target.MaxEnergy;

            energylabelMe.Content = $"{Me.Energy} / {Me.MaxEnergy}";
            energylabelTarget.Content = $"{Target.Energy} / {Target.MaxEnergy}";

            scorelabel.Content = $"Score: {ScoreMe} / {ScoreTarget}";
        }

        private void LogCombatActionMe(string text, string color = "white")
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(combatlogMe.Document.ContentEnd, combatlogMe.Document.ContentEnd)
            {
                Text = $"{text}\r"
            };

            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
            combatlogMe.ScrollToEnd();
        }

        private void LogCombatActionTarget(string text, string color = "white")
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(combatlogTarget.Document.ContentEnd, combatlogTarget.Document.ContentEnd)
            {
                Text = $"{text}\r"
            };

            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { };
            combatlogTarget.ScrollToEnd();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            InitCombatEngine();
        }

        private void ButtonDoSimulations_Click(object sender, RoutedEventArgs e)
        {
            ScoreMe = 0;
            ScoreTarget = 0;
            this.Dispatcher.Invoke(UpdateViews);
        }

        private void ButtonDoSimulations(object sender, RoutedEventArgs e)
        {
            int count = int.Parse(simulationCount.Text);
            new Thread(new ThreadStart(() => DoSimulations(count))).Start();
        }

        private void DoSimulations(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.Dispatcher.Invoke(InitCombatEngine);

                do
                {
                    DoIteration(false);
                    Thread.Sleep(10);
                } while (!FightIsOver);
            }

            this.Dispatcher.Invoke(UpdateViews);
        }

        private void SpellSelection_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActiveSpellForEnemy = (Spell)spellSelection.SelectedItem;
        }
    }
}
