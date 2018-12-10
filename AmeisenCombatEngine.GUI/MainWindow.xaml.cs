using AmeisenCombatEngineCore;
using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.FSM.Enums;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Strategies;
using AmeisenCombatEngineCore.Structs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

            Vector3 positionMe = new Vector3(150, 40, 0);
            Vector3 positionTarget = new Vector3(300, 40, 0);

            Me = new Unit(health, health, energy, energy, CombatState.Standing, positionMe);
            Target = new Unit(healthTarget, healthTarget, energyTarget, energyTarget, CombatState.Standing, positionTarget);

            this.Dispatcher.Invoke(UpdateViews);

            CombatEngine = new CombatEngine(Me, Target, Spells, new SpellSimple(Spells, 30), new MovementCloseCombat());
            CombatEngine.OnCastSpell += HandleMeCast;
            CombatEngine.OnMoveCharacter += HandleMeMove;

            CombatEngine2 = new CombatEngine(Target, Me, Spells, new SpellSimple(Spells, 70), new MovementCloseCombat());
            CombatEngine2.OnCastSpell += HandleTargetCast;
            CombatEngine2.OnMoveCharacter += HandleTargetMove;
        }

        private void HandleMeMove(object sender, EventArgs e)
        => ProcessMovement(Me, ((MoveCharacterEventArgs)e).PositionToGoTo);

        private void HandleTargetMove(object sender, EventArgs e)
        => ProcessMovement(Target, ((MoveCharacterEventArgs)e).PositionToGoTo);


        private void HandleMeCast(object sender, EventArgs e)
            => ProcessSpellUsage(((CastSpellEventArgs)e).Spell, Me, Target);


        private void HandleTargetCast(object sender, EventArgs e)
            => ProcessSpellUsage(((CastSpellEventArgs)e).Spell, Target, Me);

        private void ButtonDoIteration_Click(object sender, RoutedEventArgs e) => DoIteration();


        private void DoIteration(bool updateViews = true)
        {
            if (FightIsOver)
            {
                return;
            }

            if (Me.Health <= 0)
            {
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

            CombatEngine.DoIteration();
            CombatEngine2.DoIteration();
        }

        private void ProcessSpellUsage(Spell usedSpell, Unit a, Unit b, bool updateViews = true)
        {
            a.Energy -= usedSpell.EnergyCost;
            foreach (KeyValuePair<SpellType, double> spellImpact in usedSpell.SpellImpacts)
            {
                switch (spellImpact.Key)
                {
                    case SpellType.Damage:
                        b.Health -= spellImpact.Value;
                        this.Dispatcher.Invoke(UpdateViews);
                        break;

                    case SpellType.Heal:
                        a.Health += spellImpact.Value;
                        this.Dispatcher.Invoke(UpdateViews);
                        break;

                    default:
                        break;
                }
            }
        }

        private void ProcessMovement(Unit unit, Vector3 newPosition)
        {
            Vector3 currentPosition = unit.Position;

            if (currentPosition.X < newPosition.X)
            {
                currentPosition.X += 1;
            }

            if (currentPosition.Y < newPosition.Y)
            {
                currentPosition.Y += 1;
            }

            if (currentPosition.X > newPosition.X)
            {
                currentPosition.X -= 1;
            }

            if (currentPosition.Y > newPosition.Y)
            {
                currentPosition.Y -= 1;
            }

            unit.Position = currentPosition;
            this.Dispatcher.Invoke(UpdateViews);
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

            mainCanvas.Children.Clear();

            DrawRectangleOnCanvas(Me.Position.X, Me.Position.Y, new SolidColorBrush(Colors.White));
            DrawRectangleOnCanvas(Target.Position.X, Target.Position.Y, new SolidColorBrush(Colors.Red));
        }

        private void DrawRectangleOnCanvas(double x, double y, Brush brush)
        {
            Rectangle rect = new Rectangle
            {
                Stroke = brush,
                Fill = brush,
                Width = 4,
                Height = 4,
                StrokeThickness = 2
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            mainCanvas.Children.Add(rect);
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void ButtonExit_Click(object sender, RoutedEventArgs e) => Close();

        private void ButtonResetSimulation_Click(object sender, RoutedEventArgs e) => InitCombatEngine();


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
