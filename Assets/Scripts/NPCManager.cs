using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }
    private List<NPC> npcs = new List<NPC>();

    public delegate void NPCEvent(NPC npc, GameEventData eventData);
    public event NPCEvent OnNPCEvent;


    [Header("NPC Name Lists")]
    public List<string> maleNames = new List<string> {
"Aaron", "Abel", "Abraham", "Adam", "Adrian", "Aiden", "Alan", "Albert", "Alec", "Alex",
"Alexander", "Alfred", "Ali", "Allen", "Alvin", "Amir", "Andre", "Andrew", "Andy", "Angel",
"Anthony", "Antonio", "Archie", "Ari", "Arthur", "Asher", "Austin", "Avery", "Axel", "Barry",
"Beau", "Ben", "Benjamin", "Bennett", "Bernard", "Billy", "Blaine", "Blake", "Bo", "Bobby",
"Brad", "Bradley", "Brady", "Brandon", "Brayden", "Brent", "Brett", "Brian", "Brock", "Brody",
"Bruce", "Bruno", "Bryan", "Bryce", "Byron", "Caleb", "Calvin", "Camden", "Cameron", "Carl",
"Carlos", "Carter", "Casey", "Cecil", "Cedric", "Chad", "Chance", "Chandler", "Charles", "Charlie",
"Chase", "Chris", "Christian", "Christopher", "Clarence", "Clark", "Clay", "Clayton", "Cliff", "Clifford",
"Clyde", "Cody", "Colby", "Cole", "Colin", "Collin", "Conner", "Connor", "Conrad", "Corey",
"Cornelius", "Cory", "Craig", "Cris", "Cristian", "Curtis", "Cyrus", "Dakota", "Dale", "Dallas",
"Damian", "Damien", "Damon", "Dan", "Daniel", "Danny", "Dante", "Darian", "Darius", "Darnell",
"Darren", "Dave", "David", "Dean", "Declan", "Dennis", "Derek", "Desmond", "Devin", "Devon",
"Dexter", "Diego", "Dominic", "Don", "Donald", "Donovan", "Doug", "Douglas", "Drake", "Drew",
"Duncan", "Dustin", "Dwayne", "Dylan", "Earl", "Easton", "Eddie", "Eden", "Edgar", "Eduardo",
"Edward", "Edwin", "Eli", "Elias", "Elijah", "Elmer", "Emerson", "Emiliano", "Emilio", "Emmanuel",
"Emmett", "Enrique", "Eric", "Erick", "Erik", "Ernest", "Esteban", "Ethan", "Eugene", "Evan",
"Everett", "Ezekiel", "Ezra", "Fabian", "Felix", "Fernando", "Finn", "Floyd", "Forrest", "Francis",
"Francisco", "Frank", "Frankie", "Franklin", "Fred", "Freddie", "Frederick", "Gabriel", "Gage", "Gareth",
"Gavin", "Gene", "George", "Gerald", "Gilbert", "Glen", "Glenn", "Gordon", "Grady", "Graham",
"Grant", "Grayson", "Greg", "Gregory", "Griffin", "Guillermo", "Gus", "Guy", "Harley", "Harold",
"Harrison", "Harry", "Harvey", "Hayden", "Heath", "Hector", "Henry", "Herbert", "Herman", "Hugh",
"Hugo", "Hunter", "Ian", "Ibrahim", "Ignacio", "Isaac", "Isaiah", "Ismael", "Israel", "Ivan",
"Izaiah", "Jace", "Jack", "Jackson", "Jacob", "Jaden", "Jake", "Jakob", "Jamal", "James",
"Jameson", "Jamie", "Jared", "Jarrett", "Jarvis", "Jason", "Jasper", "Javier", "Jaxon", "Jay",
"Jayce", "Jayden", "Jean", "Jeff", "Jefferson", "Jeffrey", "Jeremiah", "Jeremy", "Jermaine", "Jerry",
"Jesse", "Jesus", "Jett", "Jimmy", "Joe", "Joel", "Joey", "John", "Johnny", "Jon",
"Jonah", "Jonathan", "Jordan", "Jorge", "Jose", "Joseph", "Josh", "Joshua", "Josiah", "Jude",
"Julian", "Julio", "Julius", "Justin", "Kade", "Kai", "Kaleb", "Kameron", "Kane", "Kareem",
"Karl", "Karson", "Kasen", "Kayden", "Keaton", "Keith", "Kellan", "Kelly", "Kelvin", "Ken",
"Kendall", "Kendrick", "Kenneth", "Kenny", "Kent", "Kerry", "Kevin", "Khalil", "Kian", "Kingston",
"Kirby", "Kirk", "Kobe", "Kolby", "Kris", "Kristian", "Kurt", "Kurtis", "Kyle", "Kyler",
"Kyleigh", "Lamar", "Lamont", "Lance", "Landen", "Landon", "Lane", "Larry", "Lawrence", "Layne",
"Lee", "Leo", "Leon", "Leonard", "Leonardo", "Leroy", "Leslie", "Levi", "Liam", "Lincoln",
"Lionel", "Logan", "Lonnie", "Louis", "Luca", "Lucas", "Lucian", "Luis", "Lukas", "Luke",
"Luther", "Mack", "Malachi", "Malcolm", "Manuel", "Marc", "Marcel", "Marco", "Marcos", "Marcus",
"Mario", "Mark", "Markus", "Marlon", "Marshall", "Martin", "Marvin", "Mason", "Mathew", "Matt",
"Matthew", "Maurice", "Max", "Maximilian", "Maximus", "Maxwell", "Melvin", "Micah", "Michael", "Micheal",
"Miguel", "Mike", "Miles", "Milton", "Mitch", "Mitchell", "Mohamed", "Mohammad", "Mohammed", "Monty",
"Morgan", "Moses", "Muhammad", "Myles", "Myron", "Nash", "Nasir", "Nathan", "Nathaniel", "Neal",
"Ned", "Neil", "Nelson", "Nestor", "Nicholas", "Nick", "Nickolas", "Nico", "Nigel", "Noah",
"Noel", "Nolan", "Norman", "Oliver", "Omar", "Orlando", "Oscar", "Otis", "Owen", "Pablo",
"Patrick", "Paul", "Pedro", "Percy", "Perry", "Pete", "Peter", "Phil", "Philip", "Phoenix",
"Porter", "Preston", "Prince", "Quentin", "Quincy", "Quinn", "Rafael", "Ralph", "Ramiro", "Ramon",
"Randall", "Randy", "Raphael", "Raul", "Ray", "Raymond", "Reed", "Reese", "Reginald", "Reid",
"Remy", "Rene", "Reuben", "Rex", "Rey", "Reynaldo", "Rhett", "Ricardo", "Richard", "Rick",
"Rickey", "Ricky", "Rico", "Riley", "Rob", "Robbie", "Robert", "Roberto", "Robin", "Rocco",
"Rodney", "Rodolfo", "Roger", "Roland", "Roman", "Ron", "Ronald", "Ronnie", "Roosevelt", "Rory",
"Ross", "Rowan", "Roy", "Royce", "Ruben", "Rudy", "Russell", "Ryan", "Ryder", "Ryker",
"Sam", "Samir", "Sammy", "Samuel", "Santiago", "Santino", "Saul", "Sawyer", "Scott", "Sean",
"Sebastian", "Sergio", "Seth", "Shane", "Shawn", "Sheldon", "Sidney", "Silas", "Simon", "Skylar",
"Skyler", "Solomon", "Spencer", "Stan", "Stanley", "Stefan", "Stephen", "Steve", "Steven", "Stuart",
"Sullivan", "Sylvester", "Taj", "Tanner", "Tate", "Taylor", "Ted", "Teddy", "Terence", "Terrance",
"Terrence", "Terry", "Tevin", "Thaddeus", "Theo", "Theodore", "Thomas", "Tim", "Timothy", "Tobias",
"Toby", "Todd", "Tom", "Tomas", "Tommy", "Tony", "Travis", "Trent", "Trenton", "Trevor",
"Trey", "Tristan", "Troy", "Truman", "Tucker", "Ty", "Tyler", "Tyrell", "Tyrese", "Tyrone",
"Tyson", "Ulises", "Ulysses", "Uriel", "Valentino", "Van", "Vance", "Vernon", "Victor", "Vince",
"Vincent", "Virgil", "Wade", "Walker", "Wallace", "Walter", "Warren", "Waylon", "Wayne", "Wes",
"Wesley", "Weston", "Wilbert", "Wilfred", "Will", "William", "Willie", "Wilson", "Winston", "Wyatt",
"Xander", "Xavier", "Yahir", "Yosef", "Zachariah", "Zachary", "Zack", "Zane", "Zayden", "Zion"
};
    public List<string> femaleNames = new List<string> {
"Aaliyah", "Abby", "Abigail", "Ada", "Adaline", "Addison", "Adelaide", "Adele", "Adeline", "Adriana",
"Agnes", "Aileen", "Ainsley", "Aisha", "Alana", "Alani", "Alanna", "Alayna", "Alexa", "Alexandra",
"Alexandria", "Alexia", "Alia", "Alice", "Alicia", "Alina", "Alisa", "Alison", "Alissa", "Allie",
"Allison", "Ally", "Alma", "Alondra", "Alycia", "Alyson", "Alyssa", "Amara", "Amari", "Amber",
"Amelia", "Amelie", "Amina", "Amira", "Amy", "Ana", "Anabel", "Anastasia", "Andrea", "Angela",
"Angelica", "Angelina", "Angie", "Anika", "Anita", "Aniya", "Ann", "Anna", "Annabel", "Annabella",
"Annabelle", "Annalise", "Anne", "Anneliese", "Annette", "Annie", "Annika", "Ansley", "Antonia", "April",
"Ariana", "Arianna", "Ariel", "Ariella", "Arlene", "Arya", "Ashlee", "Ashley", "Ashlyn", "Aspen",
"Audrey", "Aubree", "Aubrey", "Aubrielle", "Autumn", "Ava", "Avery", "Aviana", "Ayana", "Ayla",
"Bailee", "Bailey", "Barbara", "Beatrice", "Beatrix", "Becca", "Becky", "Belinda", "Bella", "Bernadette",
"Beryl", "Beth", "Bethany", "Betsy", "Betty", "Bianca", "Billie", "Blair", "Blaire", "Blake",
"Blanca", "Bonnie", "Brandi", "Brandy", "Breanna", "Brenda", "Brenna", "Briana", "Brianna", "Brielle",
"Bridget", "Bridgette", "Brittany", "Brooke", "Brooklyn", "Brynn", "Cadence", "Caitlin", "Caitlyn", "Calista",
"Callie", "Camila", "Camilla", "Camille", "Candace", "Candice", "Cara", "Carina", "Carissa", "Carla",
"Carlie", "Carmen", "Carol", "Carole", "Carolina", "Caroline", "Carolyn", "Carrie", "Casey", "Cassandra",
"Cassidy", "Cassie", "Catalina", "Catherine", "Cathleen", "Cecelia", "Cecilia", "Celeste", "Celestine", "Celia",
"Chanel", "Charity", "Charlene", "Charlotte", "Charmaine", "Chaya", "Chelsea", "Cher", "Cheryl", "Cheyenne",
"Chloe", "Christa", "Christie", "Christina", "Christine", "Ciara", "Cindy", "Claire", "Clara", "Clarissa",
"Claudia", "Colette", "Connie", "Cora", "Coral", "Coraline", "Cordelia", "Courtney", "Cristina", "Crystal",
"Cynthia", "Daisy", "Dakota", "Dalia", "Dana", "Daniela", "Daniella", "Danielle", "Daphne", "Darlene",
"Dawn", "Dayana", "Deborah", "Debra", "Delaney", "Delia", "Delilah", "Della", "Denise", "Desiree",
"Diana", "Diane", "Dianna", "Dina", "Dixie", "Dominique", "Donna", "Dora", "Doreen", "Dorothy",
"Drew", "Eden", "Edith", "Eileen", "Elaine", "Eleanor", "Elena", "Eliana", "Elinor", "Elisa",
"Elisabeth", "Elise", "Eliza", "Elizabeth", "Ella", "Elle", "Ellen", "Elliana", "Ellie", "Elodie",
"Eloise", "Elsa", "Elsie", "Elvira", "Elyse", "Ember", "Emelia", "Emelie", "Emely", "Emerald",
"Emerson", "Emilia", "Emilie", "Emily", "Emma", "Emmalee", "Emmalyn", "Erica", "Erika", "Erin",
"Esme", "Esmeralda", "Estella", "Estelle", "Esther", "Eva", "Evangeline", "Eve", "Evelyn", "Everleigh",
"Evie", "Faith", "Fallon", "Farrah", "Fatima", "Faye", "Felicia", "Felicity", "Fern", "Finley",
"Fiona", "Florence", "Frances", "Francesca", "Freya", "Gabby", "Gabriela", "Gabriella", "Gabrielle", "Gail",
"Gemma", "Genesis", "Genevieve", "Georgia", "Georgina", "Gia", "Gillian", "Gina", "Giselle", "Gloria",
"Grace", "Gracie", "Greta", "Gretchen", "Guadalupe", "Gwen", "Gwendolyn", "Hadley", "Hailee", "Hailey",
"Haleigh", "Haley", "Halle", "Hallie", "Hannah", "Harley", "Harmony", "Harper", "Harriet", "Haven",
"Hayden", "Hazel", "Heather", "Heaven", "Heidi", "Helen", "Helena", "Holly", "Hope", "Ila",
"Ileana", "Iliana", "Imani", "India", "Indie", "Ines", "Ingrid", "Irene", "Iris", "Irma",
"Isabel", "Isabela", "Isabella", "Isabelle", "Isis", "Isla", "Ivana", "Ivy", "Izabella", "Jackie",
"Jaclyn", "Jacqueline", "Jada", "Jade", "Jaelyn", "Jamila", "Jamie", "Jana", "Jane", "Janelle",
"Janessa", "Janet", "Janice", "Janie", "Jasmine", "Jasmin", "Jayda", "Jayden", "Jayla", "Jayleen",
"Jaylen", "Jaylin", "Jazlyn", "Jazmin", "Jazmine", "Jean", "Jeanette", "Jeanne", "Jeannette", "Jemima",
"Jen", "Jena", "Jenna", "Jennie", "Jennifer", "Jessica", "Jessie", "Jewel", "Jill", "Jillian",
"Jo", "Joan", "Joann", "Joanna", "Joanne", "Jocelyn", "Jodi", "Jodie", "Jordan", "Jordyn",
"Josefina", "Josephine", "Josie", "Joy", "Joyce", "Judith", "Judy", "Julia", "Juliana", "Julianna",
"Julianne", "Julie", "Juliet", "Juliette", "Julissa", "June", "Justice", "Justina", "Justine", "Kacey",
"Kaitlin", "Kaitlyn", "Kali", "Kalia", "Kallie", "Kamila", "Karina", "Karla", "Karlee", "Karly",
"Karolina", "Karyn", "Kassandra", "Kassidy", "Kate", "Katelyn", "Katelynn", "Katerina", "Katharine", "Katherine",
"Kathleen", "Kathryn", "Kathy", "Katie", "Katrina", "Katy", "Kay", "Kaya", "Kayla", "Kaylee",
"Kayleigh", "Keira", "Kelsey", "Kendall", "Kendra", "Kenna", "Kenzie", "Keri", "Kerri", "Khloe",
"Kiara", "Kiera", "Kim", "Kimber", "Kimberlee", "Kimberly", "Kinsley", "Kira", "Kirsten", "Kori",
"Krista", "Kristen", "Kristin", "Kristina", "Kristy", "Krystal", "Kyla", "Kylee", "Kylie", "Kyra",
"Lacey", "Laci", "Laila", "Laney", "Lara", "Laura", "Laurel", "Lauren", "Laurie", "Layla",
"Lea", "Leah", "Leandra", "Leann", "Leanna", "Leanne", "Leda", "Leia", "Leila", "Lena",
"Leslie", "Leticia", "Lexi", "Lexie", "Leyla", "Lia", "Liana", "Lianna", "Libby", "Lila",
"Lilah", "Lilia", "Lilian", "Liliana", "Lilith", "Lillian", "Lillie", "Lilly", "Lily", "Lina",
"Linda", "Lindsay", "Lindsey", "Lisa", "Lisette", "Livia", "Liz", "Liza", "Lizbeth", "Lizette",
"Lizzie", "Logan", "Lola", "London", "Lora", "Lorelei", "Loren", "Lorena", "Lori", "Lorna",
"Lorraine", "Louisa", "Louise", "Lucia", "Luciana", "Lucille", "Lucy", "Luisa", "Luna", "Lupita",
"Luz", "Lydia", "Lyla", "Lynda", "Lynette", "Lynn", "Mabel", "Macey", "Mackenzie", "Macy",
"Madalyn", "Maddison", "Madeline", "Madelyn", "Madison", "Mae", "Maeve", "Maggie", "Maia", "Maisie",
"Maja", "Makayla", "Makenna", "Makenzie", "Malaya", "Malia", "Mallory", "Mandy", "Manuela", "Mara",
"Marcia", "Margaret", "Margarita", "Margot", "Maria", "Mariah", "Mariam", "Mariana", "Marianna", "Maribel",
"Marie", "Marilyn", "Marina", "Marion", "Marisa", "Marisol", "Marissa", "Maritza", "Marjorie", "Marla"
};
    public List<string> otherNames = new List<string>();
    public List<string> lastNames = new List<string>
    {"Abbott", "Acosta", "Adams", "Aguilar", "Ahmed", "Alexander", "Allen", "Allison", "Alvarez", "Anderson",
"Andrews", "Armstrong", "Arnold", "Ashley", "Atkins", "Austin", "Avery", "Avila", "Ayala", "Bailey",
"Baker", "Baldwin", "Ball", "Banks", "Barber", "Barker", "Barnes", "Barnett", "Barrett", "Barron",
"Bass", "Bates", "Battle", "Bauer", "Baxter", "Beach", "Beard", "Beasley", "Beck", "Becker",
"Bell", "Bender", "Benjamin", "Bennett", "Benson", "Bentley", "Berry", "Best", "Bird", "Bishop",
"Black", "Blackburn", "Blackwell", "Blair", "Blake", "Blanchard", "Blankenship", "Blevins", "Bolton", "Bond",
"Bonner", "Booker", "Boone", "Booth", "Bowen", "Bowers", "Bowman", "Boyd", "Boyer", "Boyle",
"Bradford", "Bradley", "Bradshaw", "Brady", "Branch", "Bray", "Brennan", "Brewer", "Bridges", "Briggs",
"Bright", "Britt", "Brock", "Brooks", "Brown", "Browning", "Bruce", "Bryan", "Bryant", "Buchanan",
"Buck", "Buckley", "Bullock", "Burgess", "Burke", "Burks", "Burnett", "Burns", "Burt", "Burton",
"Bush", "Butler", "Byers", "Byrd", "Caldwell", "Cadogan", "Calhoun", "Callahan", "Camacho", "Cameron",
"Campbell", "Cannon", "Cantrell", "Cantu", "Cardenas", "Carey", "Carlson", "Carney", "Carpenter", "Carr",
"Carrillo", "Carroll", "Carson", "Carter", "Carver", "Case", "Casey", "Cash", "Castaneda", "Castillo",
"Castro", "Cervantes", "Chambers", "Chan", "Chandler", "Chang", "Chapman", "Charles", "Chase", "Chavez",
"Chen", "Cherry", "Christensen", "Christian", "Church", "Clark", "Clarke", "Clay", "Clayton", "Clements",
"Clemons", "Cleveland", "Cline", "Cobb", "Cochran", "Coffey", "Cohen", "Cole", "Coleman", "Collier",
"Collins", "Colon", "Compton", "Conley", "Conner", "Conrad", "Contreras", "Cook", "Cooke", "Cooley",
"Cooper", "Copeland", "Cortez", "Cote", "Cotton", "Cox", "Craft", "Craig", "Crane", "Crawford",
"Crosby", "Cross", "Cruz", "Cummings", "Cunningham", "Curry", "Curtis", "Dale", "Dalton", "Daniel",
"Daniels", "Daugherty", "Davenport", "David", "Davidson", "Davis", "Dawson", "Day", "Dean", "Decker",
"Dejesus", "Delacruz", "Delaney", "Deleon", "Delgado", "Dennis", "Diaz", "Dickerson", "Dickson", "Dillon",
"Dixon", "Dodson", "Dominguez", "Donaldson", "Donovan", "Dorsey", "Dotson", "Douglas", "Downs", "Doyle",
"Drake", "Durham", "Dyer", "Eaton", "Edwards", "Elliott", "Ellis", "Ellison", "Emerson", "English",
"Escobar", "Espinoza", "Estes", "Estrada", "Evans", "Everett", "Farley", "Farmer", "Farrell", "Faulkner",
"Ferguson", "Fernandez", "Fields", "Figueroa", "Finley", "Fischer", "Fisher", "Fitzgerald", "Fleming", "Fletcher",
"Flores", "Flynn", "Foley", "Forbes", "Ford", "Foreman", "Foster", "Fowler", "Fox", "Francis",
"Franco", "Frank", "Franklin", "Franks", "Frazier", "Freeman", "French", "Frost", "Fry", "Frye",
"Fuentes", "Fuller", "Fulton", "Gaines", "Gallagher", "Gallegos", "Galloway", "Gamble", "Garcia", "Gardner",
"Garner", "Garrett", "Garrison", "Garza", "Gates", "Gay", "George", "Gibbs", "Gibson", "Gilbert",
"Giles", "Gill", "Gillespie", "Gilliam", "Gilmore", "Glass", "Glenn", "Glover", "Golden", "Gomez",
"Gonzales", "Gonzalez", "Good", "Goodman", "Goodwin", "Gordon", "Gould", "Graham", "Grant", "Graves",
"Gray", "Green", "Greene", "Greer", "Gregory", "Griffin", "Griffith", "Grimes", "Gross", "Guerra",
"Guerrero", "Guthrie", "Gutierrez", "Guy", "Guzman", "Hahn", "Hale", "Haley", "Hall", "Hamilton",
"Hammond", "Hampton", "Hancock", "Haney", "Hansen", "Hanson", "Hardin", "Harding", "Hardy", "Harmon",
"Harper", "Harrington", "Harris", "Harrison", "Hart", "Harvey", "Hatfield", "Hawkins", "Hayden", "Hayes",
"Haynes", "Head", "Heath", "Hebert", "Henderson", "Hendricks", "Hendrix", "Henry", "Hensley", "Henson",
"Herman", "Hernandez", "Herrera", "Herring", "Hess", "Hester", "Hewitt", "Hickman", "Hicks", "Higgins",
"Hill", "Hines", "Hinton", "Hobbs", "Hodge", "Hodges", "Hoffman", "Hogan", "Holcomb", "Holden",
"Holder", "Holland", "Holloway", "Holman", "Holmes", "Holt", "Hood", "Hooper", "Hoover", "Hopkins",
"Hopper", "Horn", "Horne", "Horton", "House", "Houston", "Howard", "Howe", "Howell", "Hubbard",
"Huber", "Hudson", "Huff", "Huffman", "Hughes", "Hull", "Hunt", "Hunter", "Hurley", "Hurst",
"Hutchinson", "Hutton", "Hyde", "Ingram", "Irwin", "Isaacs", "Irving", "Ivey", "Jackson", "Jacobs",
"Jacobson", "James", "Jarvis", "Jefferson", "Jeffery", "Jenkins", "Jenkinson", "Jennings", "Jensen", "Jimenez",
"Johns", "Johnson", "Johnston", "Joiner", "Jones", "Jordan", "Joseph", "Joyce", "Joyner", "Juarez",
"Justice", "Kane", "Kaufman", "Keith", "Keller", "Kelley", "Kelly", "Kemp", "Kennedy", "Kent",
"Kerr", "Key", "Keys", "Kidd", "Kim", "King", "Kinney", "Kirby", "Kirk", "Kirkland",
"Klein", "Kline", "Knapp", "Knight", "Knowles", "Knox", "Koch", "Kramer", "Krause", "Kraus",
"Lamb", "Lambert", "Lancaster", "Landry", "Lane", "Lang", "Langley", "Lara", "Larsen", "Larson",
"Lawrence", "Laws", "Lawson", "Le", "Leach", "Leblanc", "Lee", "Leon", "Leonard", "Lester",
"Levine", "Levy", "Lewis", "Li", "Lindsay", "Lindsey", "Little", "Livingston", "Lloyd", "Logan",
"Long", "Lopez", "Lott", "Love", "Lowe", "Lowery", "Lucas", "Luna", "Lynch", "Lynn",
"Lyons", "Macias", "Mack", "Madden", "Maddox", "Maldonado", "Malone", "Mann", "Manning", "Marks",
"Marquez", "Marsh", "Marshall", "Martin", "Martinez", "Mason", "Massey", "Mathews", "Mathis", "Matthews",
"Maxwell", "May", "Mayer", "Maynard", "Mayo", "McBride", "McCall", "McCann", "McCarthy", "McCarty",
"McClain", "McClure", "McConnell", "McCormick", "McCoy", "McCray", "McCullough", "McDaniel", "McDonald", "McDowell",
"McFadden", "McFarland", "McGee", "McGowan", "McGuire", "McIntosh", "McKay", "McKee", "McKenzie", "McKinney",
"McKnight", "McLaughlin", "McLean", "McLeod", "McMahon", "McMillan", "McNeil", "McPherson", "Meadows", "Medina",
"Mejia"};

    private void Awake()
    {
        // Ensure that there is only one NPCManager instance.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Registers an NPC with the manager.
    /// This should be called when NPCs are spawned.
    /// </summary>
    public void RegisterNPC(NPC npc)
    {
        if (!npcs.Contains(npc))
            npcs.Add(npc);
    }

    /// <summary>
    /// Returns a list of all registered NPCs.
    /// </summary>
    public List<NPC> GetAllNPCs()
    {
        return npcs;
    }

    /// <summary>
    /// Broadcasts an event to all registered NPCs.
    /// Each NPC’s TriggerEvent method is called so that its MemorySystem records the event.
    /// </summary>
    public void TriggerEventForNPCs(GameEventData eventData)
    {
        foreach (NPC npc in npcs)
        {
            npc.TriggerEvent(eventData);
            OnNPCEvent?.Invoke(npc, eventData);
        }
    }

    /// <summary>
    /// Sends an event to a specific NPC.
    /// </summary>
    public void TriggerEventForNPC(NPC npc, GameEventData eventData)
    {
        if (npc != null)
        {
            npc.TriggerEvent(eventData);
            OnNPCEvent?.Invoke(npc, eventData);
        }
    }
}
