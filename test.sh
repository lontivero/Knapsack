PARTICIPANTS=10

dotnet build --configuration Release --output $(pwd)/bin $(pwd)/src/Knapsack.csproj
mkdir data

for nr in $(seq 3 $PARTICIPANTS); 
do 
    dotnet $(pwd)/bin/Knapsack.dll $nr > $(pwd)/data/knacksack-$nr-participants.txt
done