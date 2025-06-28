# per-entity

Example for Level-1 market data in a separate MongoDB and file collection per instrument.

## Prerequisites

- Docker (to run MongoDB locally)  
- .NET 8.0 SDK or later

## Run

1. **Start MongoDB**  

   ```bash
   docker run -d --name mongo -p 27017:27017 mongo:latest
2. **Clone & Run the App**

    ```bash
    git clone https://github.com/halilkocaoz/per-entity.git
    cd per-entity/src
    dotnet run .

# Blog

[Table/Collection-Per-Entity: Finansal Verilerde Enstrüman Bazlı Veri Barındırma Yaklaşımı](https://halilibrahimkocaoz.medium.com/table-collection-per-entity-finansal-verilerde-enstr%C3%BCman-bazl%C4%B1-veri-bar%C4%B1nd%C4%B1rma-yakla%C5%9F%C4%B1m%C4%B1-0eb97261706b)
