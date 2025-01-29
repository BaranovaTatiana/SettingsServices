using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Configuration.Db.Migrations
{
    /// <inheritdoc />
    public partial class InsertData : Migration
    {
        private static readonly string[] keyColumns = new[] { "FirstName", "MiddleName", "LastName" };
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
