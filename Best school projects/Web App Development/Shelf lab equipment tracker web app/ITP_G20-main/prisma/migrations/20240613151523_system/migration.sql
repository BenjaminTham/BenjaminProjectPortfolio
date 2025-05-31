/*
  Warnings:

  - You are about to drop the column `rfid_id` on the `Tray` table. All the data in the column will be lost.
  - You are about to drop the column `time_in_out` on the `Tray` table. All the data in the column will be lost.
  - You are about to drop the column `tray_id` on the `Tray` table. All the data in the column will be lost.
  - You are about to drop the `Post` table. If the table is not empty, all the data it contains will be lost.
  - Added the required column `epc` to the `Tray` table without a default value. This is not possible if the table is not empty.
  - Added the required column `inOutTime` to the `Tray` table without a default value. This is not possible if the table is not empty.
  - Changed the type of `status` on the `Tray` table. No cast exists, the column would be dropped and recreated, which cannot be done if there is data, since the column is required.

*/
-- CreateEnum
CREATE TYPE "InOut" AS ENUM ('IN', 'OUT');

-- CreateEnum
CREATE TYPE "Function" AS ENUM ('INVENTORY', 'READWRITE', 'GEOFENCE');

-- AlterTable
ALTER TABLE "Tray" DROP COLUMN "rfid_id",
DROP COLUMN "time_in_out",
DROP COLUMN "tray_id",
ADD COLUMN     "epc" TEXT NOT NULL,
ADD COLUMN     "inOutTime" TIMESTAMP(3) NOT NULL,
ADD COLUMN     "rackId" TEXT,
DROP COLUMN "status",
ADD COLUMN     "status" "InOut" NOT NULL;

-- DropTable
DROP TABLE "Post";

-- CreateTable
CREATE TABLE "Rack" (
    "id" TEXT NOT NULL,

    CONSTRAINT "Rack_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Reader" (
    "id" TEXT NOT NULL,
    "ip" TEXT NOT NULL,

    CONSTRAINT "Reader_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Antenna" (
    "id" TEXT NOT NULL,
    "antennaPort" INTEGER NOT NULL,
    "function" "Function" NOT NULL,
    "readerId" TEXT NOT NULL,

    CONSTRAINT "Antenna_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "SystemStatus" (
    "id" TEXT NOT NULL,
    "systemStatus" BOOLEAN NOT NULL,
    "systemTime" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "SystemStatus_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE INDEX "Antenna_readerId_idx" ON "Antenna"("readerId");

-- CreateIndex
CREATE INDEX "Tray_rackId_idx" ON "Tray"("rackId");

-- AddForeignKey
ALTER TABLE "Tray" ADD CONSTRAINT "Tray_rackId_fkey" FOREIGN KEY ("rackId") REFERENCES "Rack"("id") ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "Antenna" ADD CONSTRAINT "Antenna_readerId_fkey" FOREIGN KEY ("readerId") REFERENCES "Reader"("id") ON DELETE RESTRICT ON UPDATE CASCADE;
