using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Visual;
using ClassLibrary1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SteelColumn.Model;

namespace ClassLibrary1
{
	public class Util
	{

		int lvlNumber =3;


        public static FamilySymbol GetBeamFamilySymbolForfoundation(Autodesk.Revit.DB.Document doc, Foundation Foundation, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .Where(f => f.FamilyName == familyName)
                .FirstOrDefault();

            try
            {
                try
                {
                    ElementType fs1 = fs.Duplicate($"{Foundation.Width}" + "x" + $"{Foundation.Length} " + "X" + $"{Foundation.depth}");
                    fs1.LookupParameter("Width").Set(Util.MmToFoot(Foundation.Width));
                    fs1.LookupParameter("Length").Set(Util.MmToFoot(Foundation.Length));
                    fs1.LookupParameter("Foundation Thickness").Set(Util.MmToFoot(Foundation.depth));

                    fs = fs1 as FamilySymbol;
                    return fs;
                }
                catch (Exception)
                {

                    return new FilteredElementCollector(doc).OfCategory(category)
                        .WhereElementIsElementType()
                        .Cast<FamilySymbol>()
                        .Where(f => f.Name == $"{Foundation.Width}" + "x" + $"{Foundation.Length} " + "X" + $"{Foundation.depth}")
                        .FirstOrDefault();
                }

            }
            catch (Exception)
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                    .WhereElementIsElementType()
                    .Cast<FamilySymbol>()
                    .Where(f => f.FamilyName == familyName)
                    .FirstOrDefault();
                return fs;

            }

        }


        public Level GetLevel(Autodesk.Revit.DB.Document doc, double elevation)
		{
			Level rLevel = null;

			try
			{
				rLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
														.WhereElementIsNotElementType()
														.Cast<Level>()
														.First(lvl =>
														{

															var asdfasd = Math.Abs(lvl.Elevation);
															var asdfsdasasd = Util.MmToFoot(elevation);
															if ((Math.Abs(lvl.Elevation - Util.MmToFoot(elevation)) < 0.1))
															{
																return true;
															}
															return false;

														});
			}
			catch (Exception)
			{
				rLevel = Level.Create(doc, Util.MmToFoot(elevation));
				rLevel.LookupParameter("Name").Set(String.Format("Level {0}", lvlNumber++));
				//rLevel.LookupParameter("Name").Set($"{lvlNumber++}");

			}

			return rLevel;
		}


		//public FamilyInstance CreateFoundation(Foundation foun)
		//{

		//}


		public FamilyInstance CreateBeamInstance(Autodesk.Revit.DB.Document doc, Beam beam, FamilySymbol fs)
		{
			StructuralType stBeam = StructuralType.Beam;
			XYZ p1 = new XYZ(Util.MmToFoot(beam.Location.X), Util.MmToFoot(beam.Location.Y), Util.MmToFoot(beam.Location.Z) + Util.MmToFoot(beam.H / 2));
			XYZ p2 = p1 + beam.Length * new XYZ(Util.MmToFoot(beam.RefDirection.X), Util.MmToFoot(beam.RefDirection.Y), Util.MmToFoot(beam.RefDirection.Z));
			Line l1 = Line.CreateBound(p1, p2);

			if (!fs.IsActive)
			{
				fs.Activate();
				doc.Regenerate();
			}

			FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);

			fi.LookupParameter("Reference Level").Set(GetLevel(doc, beam.Location.Z).Id);
			return fi;
		}

		public FamilyInstance CreateSTBeamLSECInstance(Autodesk.Revit.DB.Document doc, BeamSTLSEC beam, FamilySymbol fs)
		{
			StructuralType stBeam = StructuralType.Beam;
			XYZ p1 = new XYZ(Util.MmToFoot(beam.Location.X), Util.MmToFoot(beam.Location.Y), Util.MmToFoot(beam.Location.Z + beam.Depth / 2));
			XYZ p2 = p1 + beam.Length * new XYZ(Util.MmToFoot(beam.RefDirection.X), Util.MmToFoot(beam.RefDirection.Y), Util.MmToFoot(beam.RefDirection.Z));
			Line l1 = Line.CreateBound(p1, p2);

			if (!fs.IsActive)
			{
				fs.Activate();
				doc.Regenerate();
			}
			FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);

			fi.LookupParameter("Reference Level").Set(GetLevel(doc, beam.Location.Z).Id);
			return fi;
		}
		public FamilyInstance CreateSteelBeamInstance(Document doc, BeamStISEC beam, FamilySymbol fs)
		{


			StructuralType stBeam = StructuralType.Beam;
			XYZ p1 = new XYZ(Util.MmToFoot(beam.Location.X), Util.MmToFoot(beam.Location.Y), Util.MmToFoot(beam.Location.Z) + Util.MmToFoot(beam.Depth / 2));
			XYZ p2 = p1 + beam.Length * new XYZ(Util.MmToFoot(beam.RefDirection.X), Util.MmToFoot(beam.RefDirection.Y), Util.MmToFoot(beam.RefDirection.Z));
			Line l1 = Line.CreateBound(p1, p2);
			if (!fs.IsActive)
			{
				fs.Activate();
				doc.Regenerate();
			}
			FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
			fi.LookupParameter("Reference Level").Set(GetLevel(doc, beam.Location.Z).Id);
			return fi;




		}



		public FamilyInstance CreateSteelBeamInstanceCsec(Document doc, BeamCsec beam, FamilySymbol fs)
		{


			StructuralType stBeam = StructuralType.Beam;
			XYZ p1 = new XYZ(Util.MmToFoot(beam.Location.X), Util.MmToFoot(beam.Location.Y), Util.MmToFoot(beam.Location.Z) + Util.MmToFoot(beam.Depth / 2));
			XYZ p2 = p1 + beam.Length * new XYZ(Util.MmToFoot(beam.RefDirection.X), Util.MmToFoot(beam.RefDirection.Y), Util.MmToFoot(beam.RefDirection.Z));
			Line l1 = Line.CreateBound(p1, p2);
			if (!fs.IsActive)
			{
				fs.Activate();
				doc.Regenerate();
			}
			FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
			fi.LookupParameter("Reference Level").Set(GetLevel(doc, beam.Location.Z).Id);
			return fi;




		}

		public FamilyInstance CreateSteelBeamCircularnstance(Document doc, Circular beam, FamilySymbol fs)
		{
			StructuralType stBeam = StructuralType.Beam;
			XYZ p1 = new XYZ(Util.MmToFoot(beam.Location.X), Util.MmToFoot(beam.Location.Y), Util.MmToFoot(beam.Location.Z) + Util.MmToFoot(beam.Width / 2));
			XYZ p2 = p1 + beam.Length * new XYZ(Util.MmToFoot(beam.RefDirection.X), Util.MmToFoot(beam.RefDirection.Y), Util.MmToFoot(beam.RefDirection.Z));
			Line l1 = Line.CreateBound(p1, p2);
			if (!fs.IsActive)
			{
				fs.Activate();
				doc.Regenerate();
			}
			FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
			fi.LookupParameter("Reference Level").Set(GetLevel(doc, beam.Location.Z).Id);
			return fi;
		}


		public static FamilySymbol GetBeamFamilySymbolForCon(Autodesk.Revit.DB.Document doc, Beam beam, string familyName, BuiltInCategory category)
		{
			FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.Where(f => f.FamilyName == familyName)
														.FirstOrDefault(f =>
														{
															var sad = f.LookupParameter("h").AsDouble();
															var asdad = beam.H;
															if (Math.Abs(f.LookupParameter("h").AsDouble() - Util.MmToFoot(beam.H)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("b").AsDouble() - Util.MmToFoot(beam.B)) < Util.MmToFoot(0.001))

																return true;

															return false;


														});

			if (fs != null) return fs;

			Family family = default;

			try
			{
				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);

			}
			catch (Exception)
			{
				//string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
				//family = OpenFamily(doc, directory, familyName);
				//fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

			}

			try
			{

				var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
									   WhereElementIsElementType().Cast<FamilySymbol>().ToList();


				if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(c => Math.Abs(c.LookupParameter("h").AsDouble() - Util.MmToFoot(beam.H)) < Util.MmToFoot(0.001) &&
				Math.Abs(c.LookupParameter("b").AsDouble() - Util.MmToFoot(beam.B)) < Util.MmToFoot(0.0001))))
				{



					ElementType fs1 = fs.Duplicate($"{beam.H}" + "x" + $"{beam.B} " + "X" + $"{beam.Location.X}");
					fs1.LookupParameter("h").Set(Util.MmToFoot(beam.H));
					fs1.LookupParameter("b").Set(Util.MmToFoot(beam.B));

					fs = fs1 as FamilySymbol;

				}
			}
			catch (Exception)
			{


				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);
			}


			return fs;


		}

		public static FamilySymbol GetBeamFamilySymbolForISEC(Autodesk.Revit.DB.Document doc, BeamStISEC beam, string familyName, BuiltInCategory category)
		{

			FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.Where(f => f.FamilyName == familyName)
														.FirstOrDefault(f =>
														{
															if (Math.Abs(f.LookupParameter("Web Thickness").AsDouble() * 10 - Util.MmToFoot(beam.WebTh)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("Flange Thickness").AsDouble() * 10 - Util.MmToFoot(beam.FlangeTh)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("Width").AsDouble() * 10 - Util.MmToFoot(beam.Width)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("Height").AsDouble() * 10 - Util.MmToFoot(beam.Depth)) < Util.MmToFoot(0.001))
																return true;

															return false;



														});

			if (fs != null) return fs;
			Family family = default;




			try
			{
				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);

			}
			catch (Exception)
			{
				//string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
				//family = OpenFamily(doc, directory, familyName);
				//fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

			}


			var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
								   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

			try
			{

				if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Depth}" + $"{beam.FlangeTh}" +
																							 $"{beam.Width}" + $"{beam.WebTh}" + $"{beam.Location.X}")))
				{


					ElementType fs1 = fs.Duplicate($"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}" + "X" + $"{beam.Location.X}");
					fs1.LookupParameter("Web Thickness").Set(Util.MmToFoot(beam.WebTh));
					fs1.LookupParameter("Flange Thickness").Set(Util.MmToFoot(beam.FlangeTh));
					fs1.LookupParameter("Width").Set(Util.MmToFoot(beam.Width));
					var sdad = fs1.LookupParameter("Height").Set(Util.MmToFoot(beam.Depth));
					return fs1 as FamilySymbol;
				}
			}
			catch (Exception)
			{

				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);
			}






			return fs;


		}


		public static FamilySymbol GetBeamFamilySymbolForCSEC(Autodesk.Revit.DB.Document doc, BeamCsec beam, string familyName, BuiltInCategory category)
		{

			FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.Where(f => f.FamilyName == familyName)
														.FirstOrDefault(f =>
														{
															var SADAS = f.LookupParameter("tw").AsDouble() * 10;
															var ASDA = Util.MmToFoot(beam.WebTh);
															if (Math.Abs(f.LookupParameter("tw").AsDouble() * 10 - Util.MmToFoot(beam.WebTh)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("tf").AsDouble() * 10 - Util.MmToFoot(beam.FlangeTh)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("b").AsDouble() * 10 - Util.MmToFoot(beam.Width)) < Util.MmToFoot(0.001) &&
																Math.Abs(f.LookupParameter("h").AsDouble() * 10 - Util.MmToFoot(beam.Depth)) < Util.MmToFoot(0.001))
																return true;

															return false;



														});

			if (fs != null) return fs;
			Family family = default;




			try
			{
				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);

			}
			catch (Exception)
			{
				//string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
				//family = OpenFamily(doc, directory, familyName);
				//fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

			}


			var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
								   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

			try
			{

				if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Depth}" + $"{beam.FlangeTh}" +
																							 $"{beam.Width}" + $"{beam.WebTh}" )))
				{


					ElementType fs1 = fs.Duplicate($"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}"  );
					fs1.LookupParameter("tw").Set(Util.MmToFoot(beam.WebTh));
					fs1.LookupParameter("tf").Set(Util.MmToFoot(beam.FlangeTh));
					fs1.LookupParameter("b").Set(Util.MmToFoot(beam.Width));
					var sdad = fs1.LookupParameter("h").Set(Util.MmToFoot(beam.Depth));
					return fs1 as FamilySymbol;
				}
			}
			catch (Exception)
			{

				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.Name == $"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}");
			}






			return fs;


		}






















		public static FamilySymbol GetBeamFamilySymbolForCircular(Autodesk.Revit.DB.Document doc, Circular beam, string familyName, BuiltInCategory category)
		{
			FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.Where(f => f.FamilyName == familyName)
														.FirstOrDefault(f =>
														{
															if (
																Math.Abs(f.LookupParameter("d").AsDouble() * 10 - Util.MmToFoot(beam.Width)) < Util.MmToFoot(0.001))
																return true;

															return false;



														});

			if (fs != null) return fs;
			Family family = default;


			try
			{
				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);

			}
			catch (Exception)
			{
				//string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
				//family = OpenFamily(doc, directory, familyName);
				//fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

			}


			var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
								   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

			try
			{

				if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Width}" + $"{beam.Location.X}")))

				{
					ElementType fs1 = fs.Duplicate( $"{beam.Width*10}" + "X" + $"{beam.Location.X}");

					fs1.LookupParameter("d").Set(Util.MmToFoot(beam.Width*10));

					return fs1 as FamilySymbol;
				}
			}
			catch (Exception)
			{

				fs = new FilteredElementCollector(doc).OfCategory(category)
														.WhereElementIsElementType()
														.Cast<FamilySymbol>()
														.First(f => f.FamilyName == familyName);
			}

			return fs;

		}



        public static FamilySymbol GetBeamFamilySymbolForLSEC(Autodesk.Revit.DB.Document doc, BeamSTLSEC beam, string familyName, BuiltInCategory category)
        {

            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {

                                                            var SADAS = f.LookupParameter("Wall Nominal Thickness").AsDouble();
                                                            var SAD = beam.FlangeTh;

                                                            if (
                                                                Math.Abs(f.LookupParameter("Wall Nominal Thickness").AsDouble() - Util.MmToFoot(beam.FlangeTh)) < Util.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Width").AsDouble() - Util.MmToFoot(beam.Width)) < Util.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Height").AsDouble() - Util.MmToFoot(beam.Depth)) < Util.MmToFoot(0.001))


                                                                return true;

                                                            return false;

                                                        });

            if (fs != null) return fs;
            Family family = default;

            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                //string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                //family = OpenFamily(doc, directory, familyName);
                //fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;


            }

            try
            {

                var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                                   WhereElementIsElementType().Cast<FamilySymbol>().ToList();


                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(c => c.Name == $"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}"
                                                                                        )))
                {
                    ElementType fs1 = fs.Duplicate($"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}");
                    fs1.LookupParameter("Wall Nominal Thickness").Set(Util.MmToFoot(beam.FlangeTh));
                    fs1.LookupParameter("Width").Set(Util.MmToFoot(beam.Width));
                    fs1.LookupParameter("Height").Set(Util.MmToFoot(beam.Depth));
                    return fs1 as FamilySymbol;
                }
            }
            catch (Exception)
            {

                fs = new FilteredElementCollector(doc).OfCategory(category)
                                        .WhereElementIsElementType()
                                        .Cast<FamilySymbol>()
                                        .First(f => f.FamilyName == familyName);
            }

            return fs;
        }



        /// <summary>
        /// Base units currently used internally by Revit.
        /// </summary>
        enum BaseUnit
		{
			BU_Length = 0,         // length, feet (ft)
			BU_Angle,              // angle, radian (rad)
			BU_Mass,               // mass, kilogram (kg)
			BU_Time,               // time, second (s)
			BU_Electric_Current,   // electric current, ampere (A)
			BU_Temperature,        // temperature, kelvin (K)
			BU_Luminous_Intensity, // luminous intensity, candela (cd)
			BU_Solid_Angle,        // solid angle, steradian (sr)

			NumBaseUnits
		};

		const double _inchToMm = 25.4;
		const double _footToMm = 12 * _inchToMm;
		const double _footToMeter = _footToMm * 0.001;
		const double _sqfToSqm = _footToMeter * _footToMeter;
		const double _cubicFootToCubicMeter = _footToMeter * _sqfToSqm;

		/// <summary>
		/// Convert a given length in feet to millimetres.
		/// </summary>
		public static double FootToMm(double length)
		{
			return length * _footToMm;
		}

		/// <summary>
		/// Convert a given length in feet to millimetres,
		/// rounded to the closest millimetre.
		/// </summary>
		public static int FootToMmInt(double length)
		{
			//return (int) ( _feet_to_mm * d + 0.5 );
			return (int)Math.Round(_footToMm * length,
			  MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// Convert a given length in feet to metres.
		/// </summary>
		public static double FootToMetre(double length)
		{
			return length * _footToMeter;
		}

		/// <summary>
		/// Convert a given length in millimetres to feet.
		/// </summary>
		public static double MmToFoot(double length)
		{
			return length / _footToMm;
		}

		/// <summary>
		/// Convert a given point or vector from millimetres to feet.
		/// </summary>
		public static XYZ MmToFoot(XYZ v)
		{
			return v.Divide(_footToMm);
		}

		/// <summary>
		/// Convert a given volume in feet to cubic meters.
		/// </summary>
		public static double CubicFootToCubicMeter(double volume)
		{
			return volume * _cubicFootToCubicMeter;
		}

		internal static FamilySymbol GetBeamFamilySymbolForfoundation(Document doc, Foundation fF, string v, object eLEM_CATEGORY_PARAM)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Hard coded abbreviations for the first 26
		/// DisplayUnitType enumeration values.
		/// </summary>
		public static string[] DisplayUnitTypeAbbreviation
		  = new string[] {
	  "m", // DUT_METERS = 0,
      "cm", // DUT_CENTIMETERS = 1,
      "mm", // DUT_MILLIMETERS = 2,
      "ft", // DUT_DECIMAL_FEET = 3,
      "N/A", // DUT_FEET_FRACTIONAL_INCHES = 4,
      "N/A", // DUT_FRACTIONAL_INCHES = 5,
      "in", // DUT_DECIMAL_INCHES = 6,
      "ac", // DUT_ACRES = 7,
      "ha", // DUT_HECTARES = 8,
      "N/A", // DUT_METERS_CENTIMETERS = 9,
      "y^3", // DUT_CUBIC_YARDS = 10,
      "ft^2", // DUT_SQUARE_FEET = 11,
      "m^2", // DUT_SQUARE_METERS = 12,
      "ft^3", // DUT_CUBIC_FEET = 13,
      "m^3", // DUT_CUBIC_METERS = 14,
      "deg", // DUT_DECIMAL_DEGREES = 15,
      "N/A", // DUT_DEGREES_AND_MINUTES = 16,
      "N/A", // DUT_GENERAL = 17,
      "N/A", // DUT_FIXED = 18,
      "%", // DUT_PERCENTAGE = 19,
      "in^2", // DUT_SQUARE_INCHES = 20,
      "cm^2", // DUT_SQUARE_CENTIMETERS = 21,
      "mm^2", // DUT_SQUARE_MILLIMETERS = 22,
      "in^3", // DUT_CUBIC_INCHES = 23,
      "cm^3", // DUT_CUBIC_CENTIMETERS = 24,
      "mm^3", // DUT_CUBIC_MILLIMETERS = 25,
      "l" // DUT_LITERS = 26,
		  };



	}
}
