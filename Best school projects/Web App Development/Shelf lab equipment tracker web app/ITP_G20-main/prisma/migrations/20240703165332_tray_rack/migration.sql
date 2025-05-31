/*
  Warnings:

  - The primary key for the `Rack` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - The `id` column on the `Rack` table would be dropped and recreated. This will lead to data loss if there is data in the column.
  - The primary key for the `Tray` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - The `id` column on the `Tray` table would be dropped and recreated. This will lead to data loss if there is data in the column.
  - The `rackId` column on the `Tray` table would be dropped and recreated. This will lead to data loss if there is data in the column.

*/
-- DropForeignKey
ALTER TABLE "Tray" DROP CONSTRAINT "Tray_rackId_fkey";

-- AlterTable
ALTER TABLE "Rack" DROP CONSTRAINT "Rack_pkey",
DROP COLUMN "id",
ADD COLUMN     "id" SERIAL NOT NULL,
ADD CONSTRAINT "Rack_pkey" PRIMARY KEY ("id");

-- AlterTable
ALTER TABLE "Tray" DROP CONSTRAINT "Tray_pkey",
DROP COLUMN "id",
ADD COLUMN     "id" SERIAL NOT NULL,
DROP COLUMN "rackId",
ADD COLUMN     "rackId" INTEGER,
ADD CONSTRAINT "Tray_pkey" PRIMARY KEY ("id");

-- CreateIndex
CREATE INDEX "Tray_rackId_idx" ON "Tray"("rackId");

-- AddForeignKey
ALTER TABLE "Tray" ADD CONSTRAINT "Tray_rackId_fkey" FOREIGN KEY ("rackId") REFERENCES "Rack"("id") ON DELETE SET NULL ON UPDATE CASCADE;
