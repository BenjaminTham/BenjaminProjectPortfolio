/*
  Warnings:

  - Made the column `inOutTime` on table `Tray` required. This step will fail if there are existing NULL values in that column.

*/
-- AlterTable
ALTER TABLE "Tray" ALTER COLUMN "inOutTime" SET NOT NULL;
