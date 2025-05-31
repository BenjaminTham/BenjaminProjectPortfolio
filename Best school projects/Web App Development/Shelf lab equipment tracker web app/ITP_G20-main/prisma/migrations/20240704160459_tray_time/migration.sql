/*
  Warnings:

  - You are about to drop the column `inOutTime` on the `Tray` table. All the data in the column will be lost.
  - You are about to drop the column `outOfBoundTime` on the `Tray` table. All the data in the column will be lost.

*/
-- AlterTable
ALTER TABLE "Tray" DROP COLUMN "inOutTime",
DROP COLUMN "outOfBoundTime",
ADD COLUMN     "statusChg" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP;
