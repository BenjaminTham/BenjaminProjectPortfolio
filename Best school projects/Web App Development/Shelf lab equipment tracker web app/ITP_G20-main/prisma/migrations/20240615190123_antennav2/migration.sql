-- DropForeignKey
ALTER TABLE "Antenna" DROP CONSTRAINT "Antenna_readerId_fkey";

-- AddForeignKey
ALTER TABLE "Antenna" ADD CONSTRAINT "Antenna_readerId_fkey" FOREIGN KEY ("readerId") REFERENCES "Reader"("id") ON DELETE CASCADE ON UPDATE CASCADE;
