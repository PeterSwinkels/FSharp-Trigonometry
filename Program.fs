//This module's imported namespaces.
open System
open System.Diagnostics
open System.Drawing
open System.Globalization
open System.Reflection
open System.Threading
open System.Windows.Forms

//The global objects and variables used by this module.
let GraphicsO = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height) //Contains this window's graphics.
let ProgramInformation = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)           //Contains this program's information.

//This procedure displays the trigonometric values.
let DisplayTrigonometricValues (opposite:Point) hypotenuse (window:Form) =
   let Canvas = Graphics.FromImage(GraphicsO)
   let MyCosine = Math.Min(opposite.X |> float, hypotenuse) / Math.Max(opposite.X |> float, hypotenuse)
   let MySine = Math.Min(opposite.Y |> float, hypotenuse) / Math.Max(opposite.Y |> float, hypotenuse)
   let Asine = Math.Asin(MySine)
   let Angle = (if (MySine <= 0.0 && MyCosine <= 0.0) || (MySine >= 0.0 && MyCosine <= 0.0) then Math.PI - Asine
                else if MySine <= 0.0 && MyCosine >= 0.0 then (Math.PI * 2.0) + Asine
                else if MySine >= 0.0 && MyCosine >= 0.0 then Asine
                else 1.0) * (180.0 / Math.PI) |> int
   let MyTangent = MySine / MyCosine

   Canvas.DrawString("Sine: " + MySine.ToString("0.00"), window.Font, Brushes.Red, new PointF(0.0F, 0.0F))
   Canvas.DrawString("Cosine: " + MyCosine.ToString("0.00"), window.Font, Brushes.Yellow, new PointF(0.0F, 16.0F))
   Canvas.DrawString("Tangent: " + MyTangent.ToString("0.00"), window.Font, Brushes.Green, new PointF(0.0F, 32.0F))
   Canvas.DrawString("Hypotenuse: " + (hypotenuse |> int).ToString(), window.Font, Brushes.Blue, new PointF(0.0F, 48.0F))
   Canvas.DrawString("Opposite: " + opposite.Y.ToString(), window.Font, Brushes.Red, new PointF(0.0F, 64.0F))
   Canvas.DrawString("Adjacent: " + opposite.X.ToString(), window.Font, Brushes.Yellow, new PointF(0.0F, 80.0F))
   Canvas.DrawString("Angle: " + Angle.ToString(), window.Font, Brushes.Green, new PointF(0.0F, 96.0F))
   
//This function draws the graphics.
let DrawGraphics oppositeX oppositeY (window:Form) = 
   let Canvas = Graphics.FromImage(GraphicsO)
   let Center = new Point(window.ClientSize.Width / 2, window.ClientSize.Height / 2)
   let Opposite = new Point(oppositeX - Center.X, oppositeY - Center.Y)
   let Hypotenuse = Math.Sqrt(((Opposite.X |> float) ** 2.0) + ((Opposite.Y |> float) ** 2.0))

   Canvas.Clear(window.BackColor)
   Canvas.DrawLine(Pens.Green, Center.X, 0, Center.X, window.ClientSize.Height)
   Canvas.DrawLine(Pens.Green, 0, Center.Y, window.ClientSize.Width, Center.Y)
   Canvas.DrawLine(Pens.Red, Center.X + Opposite.X, Center.Y + Opposite.Y, Center.X + Opposite.X, Center.Y)
   Canvas.DrawLine(Pens.Yellow, Center.X + Opposite.X, Center.Y, Center.X, Center.Y)
   Canvas.DrawLine(Pens.Blue, Center.X + Opposite.X, Center.Y + Opposite.Y, Center.X, Center.Y)
   Canvas.DrawEllipse(Pens.Purple, Center.X - (Math.Abs(Hypotenuse) |> int), Center.Y - (Math.Abs(Hypotenuse) |> int), (Math.Abs(Hypotenuse) |> int) * 2, (Math.Abs(Hypotenuse) |> int) * 2)

   DisplayTrigonometricValues Opposite Hypotenuse window
   window.Invalidate()   

//This class contains this program's main interface window.
type InterfaceWindow() as Form = 
   inherit Form()
   do Form.InitializeForm

   //This procedure initializes this window.
   member this.InitializeForm = 
      this.BackColor <- Color.Black 
      this.BackgroundImage <- GraphicsO
      this.Font <- new Font("MS Sans Serif", 12.0F)
      this.Height <- (((Screen.PrimaryScreen.WorkingArea.Height |> float) / 1.1) |> int)
      this.MouseClick.AddHandler(new MouseEventHandler (fun s e -> this.Form_MouseClick(s, e)))
      this.Resize.AddHandler(new EventHandler (fun s e -> this.Form_Resize(s, e)))
      this.StartPosition <- FormStartPosition.CenterScreen 
      this.Text <- ProgramInformation.ProductName + " v" + ProgramInformation.ProductVersion + " by: " + ProgramInformation.CompanyName 
      this.Width <- (((Screen.PrimaryScreen.WorkingArea.Width |> float) / 1.1) |> int)      
      
      DrawGraphics (this.ClientSize.Width / 2) (this.ClientSize.Height / 2) this

   //This procedure handles the user's mouse clicks.
   member this.Form_MouseClick(sender:Object, e:MouseEventArgs) =       
      DrawGraphics e.X e.Y this
      
   //This procedure adjusts this window's objects to its new size.
   member this.Form_Resize(sender:Object, e:EventArgs) = 
      this.Invalidate()

//This procedure is executed when this program is started.
[<STAThread>]
Thread.CurrentThread.CurrentCulture <- new CultureInfo("en-US")
do Application.Run(new InterfaceWindow())
