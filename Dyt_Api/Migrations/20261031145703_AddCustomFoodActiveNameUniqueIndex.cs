using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyt_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomFoodActiveNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "UX_CustomFoods_ActiveName"
                ON "CustomFoods" ("CreatedByUserId", lower(replace(replace("Name", 'İ', 'i'), 'I', 'i')))
                WHERE "IsActive";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP INDEX "UX_CustomFoods_ActiveName";""");
        }
    }
}
