using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWiki.Data.EntityFramework.Migrations
{
    public partial class IntroducedArticleProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "ArticleProperties",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ArticleProperties",
                table: "Articles");


        }
    }
}
