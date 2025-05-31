/*
  Warnings:

  - The `status` column on the `Tray` table would be dropped and recreated. This will lead to data loss if there is data in the column.

*/
-- CreateEnum
CREATE TYPE "TrayStatus" AS ENUM ('IN', 'OUT', 'OUT_OF_BOUND');

-- AlterTable
ALTER TABLE "Tray" ADD COLUMN     "outOfBoundTime" TIMESTAMP(3),
DROP COLUMN "status",
ADD COLUMN     "status" "TrayStatus" NOT NULL DEFAULT 'OUT';

-- DropEnum
DROP TYPE "InOut";
