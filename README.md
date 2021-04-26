---

จัดการ Log ด้วย ElasticSearch, Kibana, DotNetCore WebApi
การจัดเก็บ Log ด้วย ElasticSearch, Kibana, DotNetCore WebApi, Serilog
สิ่งที่ต้องติดตั้งก่อนเริ่ม VS Code, Docker
1.) เริ่มต้นจากการ Run Elasticsearch และ Kibana containers ขึ้นมาโดยสร้าง docker-compose.yml File ตาม Location นี้
mkdir -p elastic-kibana/src/docker
cd elastic-kibana/src/docker

version: '3.1'

services:

  elasticsearch:
   container_name: elasticsearch
   image: docker.elastic.co/elasticsearch/elasticsearch:7.9.2
   ports:
    - 9200:9200
   volumes:
    - elasticsearch-data:/usr/share/elasticsearch/data
   environment:
    - xpack.monitoring.enabled=true
    - xpack.watcher.enabled=false
    - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    - discovery.type=single-node
   networks:
    - elastic

  kibana:
   container_name: kibana
   image: docker.elastic.co/kibana/kibana:7.9.2
   ports:
    - 5601:5601
   depends_on:
    - elasticsearch
   environment:
    - ELASTICSEARCH_URL=http://localhost:9200
   networks:
    - elastic
  
networks:
  elastic:
    driver: bridge

volumes:
  elasticsearch-data:
บันทึก File แล้ว Run Command
docker-compose up -d
รอสักครู่ แล้วเข้าไปที่ http://localhost:9200, http://localhost:5601 หากทั้ง 2 Path run ได้ก็ถือว่าสำเร็จในขั้นตอนนี้ 🍟

---

2.) Write Log จาก .NET Core ไปยัง Elasticsearch
จากขั้นตอนแรกตอนนี้เรามี Elasticsearch และ Kibana containers run อยู่เรียบร้อยแล้ว เราสามารถ write log ไป Elasticsearch ได้จาก .NET Core WebApi ของเราเลย
สร้าง webapi อยู่ใน path elastic-kibana/src/ จากขั้นตอนแรก
dotnet new webapi --no-https -o Webapi-Serilog
cd Webapi-Serilog
Add Nuget Packages to the Project
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Debug
dotnet add package Serilog.Sinks.Elasticsearch
dotnet add package Serilog.Enrichers.Environment
แก้ไข File appsettings.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    }
  },
  "ElasticConfiguration": {
    "Uri": "http://localhost:9200"
  },
  "AllowedHosts": "*"
}
Configuring in Program.cs

แก้ไขเสร็จแล้วก็ สั่ง Run api ได้แลย 😎
dotnet run

---

3.) หลังจากนั้น เราเข้าไป สร้าง Index Pattern ที่ Kibana
http://localhost:5601/app/management/kibana/indexPatterns/creat
ทำตามขั้นตอนต่อไปนี้
Index pattern name[webapi-serilog-*] -> Click[Next step] -> Time field[@timestamp] -> Click [Create Index Pattern]

---

4.) ต่อมาก็มา Write Log จาก Controller 
สร้าง File KibanaLogController.cs

เสร็จแล้วก็ 🍜🍜
dotnet run
http://localhost:5000/KibanaLog/CreateInformationLog
http://localhost:5000/KibanaLog/CreateInformationLogกลับมาที่ Kibana ->  http://localhost:5601/app/discover
ในช่อง Search พิมพ์ค้นหาตามนี้ 
message: "_Log Testing"หรือ  level:"Information"
ก็จะเจอ Log ที่เราเขียนลงไปเป็นอันเสร็จเรียบร้อยครับผม 🎉🎉🎉🎉

---

คุณสามารถ เพิ่ม Package ต่างๆเพื่อเพิ่มความสามารถได ้เช่น 
 - Serilog.Exceptions Nuget Package
สามารถ ตั้งค่า Level ที่ต้องการให้ บันทึก Log ได้เพื่อลดความวุ่นวายของ Log ที่เราบันทึกลงไป ศึกษาเพิ่มเติมได้ที่ด้านล่างนี้เลยครับ
Resources
ElasticSeach
Kibana
Serilog
logging-with-elasticsearch-kibana-asp-net-core-and-docker