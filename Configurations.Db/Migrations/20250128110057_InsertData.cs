using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Configurations.Db.Migrations
{
    /// <inheritdoc />
    public partial class InsertData : Migration
    {
        private static object[] _colors =  {
            "Red",
            "Blue",
            "Yellow",
            "Green",
            "Black",
            "White",
            "Brown",
            "Orange",
            "Purple",
            "Pink",
            "Gray"
        };
        
        private static readonly string[] keyColumns = new[] { "FirstName", "MiddleName", "LastName" };

        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var color in _colors)
            {
                migrationBuilder.InsertData(
                    table: "Colors",
                    columns: new[] { "ColorName" },
                    values: new object[] { color }
                );
            }
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: keyColumns,
                values: new object[]
                {
                    "Аркадий",
                    "Альбертович",
                    "Иванов"
                });
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: keyColumns,
                values: new object[]
                {
                    "Алла",
                    "Макаровна",
                    "Григорьева"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var color in _colors)
            {
                migrationBuilder.DeleteData(table: "Colors", "ColorName", color);
            }

            migrationBuilder.DeleteData(table: "Users", 
                keyColumns, 
                new object[]
                {
                    "Алла",
                    "Макаровна",
                    "Григорьева"
                });
            
            migrationBuilder.DeleteData(table: "Users", 
                keyColumns, 
                new object[]
                {
                    "Аркадий",
                    "Альбертович",
                    "Иванов"
                });
        }
    }
}
