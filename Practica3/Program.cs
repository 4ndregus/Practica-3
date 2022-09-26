using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.IO;
using Practica3.Estructura;
using Practica3.Modelo;
using Practica3.Compresión;


namespace Practica3
{
    public class Program
    {
        public static AVL<Persona> AVLDpi;
        public static Compresion compresion = new Compresion();
        public static List<List<string>> listaCodificados = new List<List<string>>();
        static void Main(string[] args)
        {
            string ubicacionArchivo = "C:\\Users\\agust\\OneDrive - Universidad Rafael Landivar\\URL\\6) Segundo Ciclo 2022\\Estructura de datos II\\Practica-3\\Practica3\\input.csv";
            StreamReader archivo = new StreamReader(ubicacionArchivo);
            string linea;

            AVLDpi = new AVL<Persona>();
            while ((linea = archivo.ReadLine()) != null)
            {
                string[] fila = linea.Split(';'); //Separador

                if (fila[0] == "INSERT")
                {
                    string json = fila[1];
                    Persona nuevaPersona = JsonSerializer.Deserialize<Persona>(json);

                    int i = 0;
                    //Buscar las cartas para cada persona
                    DirectoryInfo dir = new DirectoryInfo(@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\inputs");
                    foreach (var value in dir.GetFiles($"REC-{nuevaPersona.dpi}-?.txt"))
                    {
                        nuevaPersona.cartas.Add(i + ": " + value.Name);
                        i++;
                    }

                    //A cada persona se le inserta un arreglo de todas las cartas que se encontraron
                    AVLDpi.insertar(nuevaPersona, nuevaPersona.CompararDpi);
                }
                if (fila[0] == "DELETE")
                {
                    string json = fila[1];
                    Persona nuevaPersona = JsonSerializer.Deserialize<Persona>(json);
                    AVLDpi.eliminar(nuevaPersona, nuevaPersona.CompararDpi);
                }
                if (fila[0] == "PATCH")
                {
                    string json = fila[1];
                    Persona nuevaPersona = JsonSerializer.Deserialize<Persona>(json);
                    Nodo<Persona> nuevoNodo = new Nodo<Persona>(nuevaPersona);

                    int i = 0;
                    //Buscar las cartas para cada persona
                    DirectoryInfo dir = new DirectoryInfo(@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\inputs");
                    foreach (var value in dir.GetFiles($"REC-{nuevaPersona.dpi}-?.txt"))
                    {
                        nuevaPersona.cartas.Add(i + ": " + value.Name);
                        i++;
                    }

                    AVLDpi.modificar(nuevoNodo, nuevaPersona.CompararDpi);

                }
            }

            Buscar();
            Console.ReadKey();
        }

        public static void Buscar()
        {
            string dpi;
            Console.Clear();
            Console.Write("Ingrese DPI a buscar: ");
            dpi = Console.ReadLine();

            Persona busqueda = new Persona("", dpi, "", "");
            AVLDpi.buscar(busqueda, busqueda.CompararDpi);

            if (AVLDpi.listaBusqueda.Count() == 0)
            {
                Console.WriteLine("Persona no encontrada");
            }
            else
            {
            Menu:
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"PERSONA ENCONTRADA:\n");

                //Mostar json de la persona
                var options = new JsonSerializerOptions
                {
                    IgnoreReadOnlyProperties = true,
                    WriteIndented = true
                };

                string jsonl = JsonSerializer.Serialize(AVLDpi.listaBusqueda[0], options);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(jsonl);
              
                try
                {
                    int opcion;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("");
                    Console.WriteLine(" -------------  Menú de Vistas  ------------ ");
                    Console.WriteLine("|            1.Comprimir cartas             |");
                    Console.WriteLine("|           2.Descomprimir cartas           |");
                    Console.WriteLine("|         3.Ver cartas comprimidas          |");
                    Console.WriteLine("|       4.Ver cartas descomprimidas         |");
                    Console.WriteLine(" ------------------------------------------- ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Selecciona opción: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    opcion = int.Parse(Console.ReadLine());

                    if (opcion == 1) //Comprimir cartas
                    {
                        //Buscar las cartas para cada persona
                        DirectoryInfo dir = new DirectoryInfo(@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\inputs");
                        foreach (var item in dir.GetFiles($"REC-{dpi}-?.txt"))
                        {
                            string codificado = "";
                            List<string> listCodificado = new List<string>();
                            List<string> entradas = new List<string>();
                            List<string> aux = new List<string>();

                            string textfile = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\inputs\{item.Name}";
                            string guardar = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\compressed\{item.Name}";

                            //Extraer texto del archivo
                            string text = "";
                            if (File.Exists(textfile)) { text = File.ReadAllText(textfile); }

                            listCodificado = compresion.codificar(text);
                            codificado = listCodificado.Aggregate((x, y) => x + y);
                            aux.Add(codificado);
                            entradas = compresion.listEntradas(text);

                            List<string>[] tablaEntradas = new List<string>[] { aux, entradas};
                            AVLDpi.listaBusqueda[0].entradas.Add(tablaEntradas);
                            List<string>[] tablaCodificacion = new List<string>[] { aux, listCodificado};
                            AVLDpi.listaBusqueda[0].codificados.Add(tablaCodificacion);

                            File.WriteAllText(guardar, codificado);
                            
                        }

                        Nodo<Persona> nuevoNodo = new Nodo<Persona>(AVLDpi.listaBusqueda[0]);
                        AVLDpi.modificar(nuevoNodo, AVLDpi.listaBusqueda[0].CompararDpi);
                        Console.Clear();
                        goto Menu;
                        
                    }
                    else if (opcion == 2) //Descomprimir cartas
                    {
                        //Buscar las cartas para cada persona
                        DirectoryInfo dir = new DirectoryInfo(@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\compressed");
                        foreach (var item in dir.GetFiles($"REC-{dpi}-?.txt"))
                        {
                            string decodificado = "";
                            string textfile = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\compressed\{item.Name}";
                            string guardar = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\unzipped\{item.Name}";

                            string text = "";
                            if (File.Exists(textfile)) { text = File.ReadAllText(textfile); }

                            for (int i = 0; i < AVLDpi.listaBusqueda[0].codificados.Count; i++)
                            {
                                if(AVLDpi.listaBusqueda[0].codificados[i][0][0] == text && AVLDpi.listaBusqueda[0].entradas[i][0][0] == text)
                                {
                                    decodificado = compresion.decodificar(AVLDpi.listaBusqueda[0].codificados[i][1], AVLDpi.listaBusqueda[0].entradas[i][1]);
                                }
                            }
                            File.WriteAllText(guardar, decodificado);
                        }
                        Console.Clear();
                        goto Menu;
                    }
                    else if (opcion == 3) //Ver cartas comprimidas
                    {
                        pedirCarta:
                        string carta = "";
                        int opcCarta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Selecciona carta: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        opcCarta = int.Parse(Console.ReadLine());

                        if(opcCarta > AVLDpi.listaBusqueda[0].cartas.Count - 1)
                        {
                            Console.WriteLine("Selecciona una carta dentro del rango");
                            goto pedirCarta;
                        }
                        carta = AVLDpi.listaBusqueda[0].cartas[opcCarta].Substring(AVLDpi.listaBusqueda[0].cartas[opcCarta].IndexOf(":") + 2);
                        string ruta = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\compressed\{carta}";
                        Process.Start("explorer.exe", ruta);
                        Console.Clear();
                        goto Menu;
                    }
                    else if (opcion == 4) //Ver cartas descomprimidas
                    {
                        pedirCarta:
                        string carta = "";
                        int opcCarta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Selecciona carta: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        opcCarta = int.Parse(Console.ReadLine());

                        if (opcCarta > AVLDpi.listaBusqueda[0].cartas.Count - 1)
                        {
                            Console.WriteLine("Selecciona una carta dentro del rango");
                            goto pedirCarta;
                        }
                        carta = AVLDpi.listaBusqueda[0].cartas[opcCarta].Substring(AVLDpi.listaBusqueda[0].cartas[opcCarta].IndexOf(":") + 2);
                        string ruta = $@"C:\Users\agust\OneDrive - Universidad Rafael Landivar\URL\6) Segundo Ciclo 2022\Estructura de datos II\Practica-3\Practica3\unzipped\{carta}";
                        Process.Start("explorer.exe", ruta);
                        Console.Clear();
                        goto Menu;
                    }
                    else
                    {
                        Console.WriteLine("Opción no válida");
                        goto Menu;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Opción no válida");
                    goto Menu;
                }
                
                
            }
        }
    }
}
