/*
  Warnings:

  - Added the required column `location` to the `Rack` table without a default value. This is not possible if the table is not empty.

*/
-- AlterTable
ALTER TABLE "Rack" ADD COLUMN     "location" TEXT NOT NULL;
