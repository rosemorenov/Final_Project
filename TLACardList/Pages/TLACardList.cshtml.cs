using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;


namespace TLACardList.Pages
{
    // This is a PageModel for a Razor Page that handles displaying TLA Card List
    public class TLACardListModel : PageModel
    {
        // Property that will store the selected card ID from form submissions
        [BindProperty]
        public int SelectedCardId { get; set; }


        // List that will hold all cards for the dropdown selection
        public List<SelectListItem> CardList { get; set; }

        // Property that will store the currently selected card object
        public Card SelectedCard { get; set; }

        // Handles HTTP GET requests to the page - loads the list of cards
        public void OnGet()
        {
            LoadCardList();
            SelectedCard = GetCardById(SelectedCardId);
        }


        // Handles HTTP POST requests (when user selects a card) - loads the card list
        // and retrieves the selected card's details when you click the show card button
        public IActionResult OnPostShow()
        {
            LoadCardList();
            if (SelectedCardId != 0)
            {
                SelectedCard = GetCardById(SelectedCardId);
            }
            return Page();
        }


        // Helper method that loads the list of cards from the SQLite database
        // for displaying in a dropdown menu
        private void LoadCardList()
        {
            CardList = new List<SelectListItem>();
            using (var connection = new SqliteConnection("Data Source=TLACardList.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM TLACardList";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CardList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // card ID as the value
                            Text = reader.GetString(1)             // card name as the display text
                        });
                    }
                }
            }
        }


        // Helper method that retrieves a specific card by its ID from the database
        // Returns all details of the card
        private Card? GetCardById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=TLACardList.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM TLACardList WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security

                // Get a reader to read the resultset from the database
                using (var reader = command.ExecuteReader())
                {
                    // If reader returns a record, then we'll return that new card from the database
                    if (reader.Read())
                    {
                        return new Card
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CardSet = reader.GetString(2),
                            CardType = reader.GetString(3),
                            CollectorNumber = reader.GetDecimal(4),
                            Cost = reader.GetString(5),
                            Details = reader.GetString(6),
                            ImageFileName = reader.GetString(7)
                        };
                    } // end if
                } // end using
            } // end using

            // if there was no card matching the ID in the database return null
            return null;
        } // end get card helper method
    }


    // Simple model class representing a card
    // model class maps to a table in the database so that you can create objects
    // in your application that represent the instance of an entity (rows in the table)
    // in the database
    public class Card
    {
        // The card's attributes match the columns in the Card table
        public int Id { get; set; }
        public string Name { get; set; }
        public string CardSet { get; set; }
        public string CardType { get; set; }
        public decimal CollectorNumber { get; set; }
        public string Cost {get; set; }
        public string Details {get; set; }
        public string ImageFileName { get; set; }
    } // end card class
}    