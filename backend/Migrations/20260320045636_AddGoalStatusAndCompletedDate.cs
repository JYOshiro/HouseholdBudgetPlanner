using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseholdBudgetApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalStatusAndCompletedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "SavingsGoals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SavingsGoals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SavingsGoals");
        }
    }
}
