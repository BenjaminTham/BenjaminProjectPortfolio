/*
  Warnings:

  - A unique constraint covering the columns `[ip]` on the table `Reader` will be added. If there are existing duplicate values, this will fail.

*/
-- CreateIndex
CREATE UNIQUE INDEX "Reader_ip_key" ON "Reader"("ip");
