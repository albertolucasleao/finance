using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFKTransactionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Transactions_TransactionId",
                table: "TransactionHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Transactions_TransactionId",
                table: "TransactionHistories",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Transactions_TransactionId",
                table: "TransactionHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Transactions_TransactionId",
                table: "TransactionHistories",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
