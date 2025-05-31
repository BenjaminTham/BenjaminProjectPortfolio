/*
  Warnings:

  - You are about to drop the column `time_returned` on the `Tray` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "Tray" DROP COLUMN "time_returned",
ADD COLUMN     "time_in_out" TIMESTAMP(3);
