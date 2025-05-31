/*
  Warnings:

  - A unique constraint covering the columns `[epc]` on the table `Tray` will be added. If there are existing duplicate values, this will fail.

*/
-- CreateIndex
CREATE UNIQUE INDEX "Tray_epc_key" ON "Tray"("epc");
