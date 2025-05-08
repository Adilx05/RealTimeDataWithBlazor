# 📡 RealTimeDataWithBlazor

A real-time dashboard web application built with **Blazor WebAssembly**, **.NET Core API**, **SignalR**, and **PostgreSQL**.

---

## 🚀 Features

- 🔌 Real-time updates using PostgreSQL `LISTEN/NOTIFY` + SignalR
- 📈 Live chart rendering with [MudBlazor](https://mudblazor.com/)
- 🌐 REST API support for fetching historical data
- ♻️ Seamless UI updates without page reloads
- 🧪 Random test data generation

---

## 🧱 Stack

| Layer          | Tech                         |
|----------------|------------------------------|
| Frontend       | Blazor WebAssembly + MudBlazor |
| Backend API    | ASP.NET Core Web API         |
| Real-time Comm | SignalR                      |
| Database       | PostgreSQL                   |
| Integration    | Npgsql + BackgroundService   |

---

## 🗂️ Project Structure

```
RealTimeDataWithBlazor/
│
├── RealTimeData.API/        → ASP.NET Core API with SignalR + PostgreSQL listener
├── RealTimeData.Web/        → Blazor WebAssembly frontend (charts, SignalR client)
```

---

## ⚙️ Setup

1. Start PostgreSQL (e.g. using Docker):

   ```bash
   docker run --name realtime-postgres -e POSTGRES_PASSWORD=1234 -p 5432:5432 -d postgres
   ```

2. Configure your `appsettings.json` connection string to match your DB.

3. Create the data table:

   ```sql
   CREATE TABLE data_points (
       id SERIAL PRIMARY KEY,
       timestamp TIMESTAMPTZ NOT NULL,
       value NUMERIC NOT NULL,
       category TEXT NOT NULL
   );
   ```

4. Add trigger for real-time notifications:

   ```sql
   CREATE OR REPLACE FUNCTION notify_new_data()
   RETURNS trigger AS $$
   DECLARE
     payload JSON;
   BEGIN
     payload = json_build_object(
       'timestamp', NEW.timestamp,
       'value', NEW.value,
       'category', NEW.category
     );
     PERFORM pg_notify('new_data_notification', payload::text);
     RETURN NEW;
   END;
   $$ LANGUAGE plpgsql;

   CREATE TRIGGER on_data_insert
   AFTER INSERT ON data_points
   FOR EACH ROW
   EXECUTE FUNCTION notify_new_data();
   ```

---

## ▶️ Running the App

1. Run `RealTimeData.API`
2. Run `RealTimeData.Web`
3. Navigate to the web app (e.g. `https://localhost:7012`)

---

## ⚠️ Developer Notes

- `DataPoint` model on the client **must** use `[JsonPropertyName("...")]` to correctly deserialize SignalR payloads.
- SignalR sends serialized JSON, not typed objects — this is essential in Blazor WebAssembly.
- `StateHasChanged()` is used after every new point to refresh the UI.
- Test data can be added via the **"Generate Random Data Point"** button.

---

## 📸 Screenshot

> Real-time dashboard view with chart and summary cards.  
> *(You can insert an image here)*

---

## 📄 License

MIT License — free for personal and commercial use.
