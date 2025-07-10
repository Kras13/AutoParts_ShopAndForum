from sklearn.ensemble import RandomForestRegressor
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_squared_error
import joblib
import pandas as pd
import pymssql
from datetime import datetime

DB_CONFIG = {
    'server': 'localhost',
    'user': 'SA',
    'password': 'SQL_SERVER_12',
    'database': 'AutoParts_ShopAndForum',
}

def get_sales_data():
    conn = pymssql.connect(**DB_CONFIG)
    cursor = conn.cursor(as_dict=True)

    query = """
    SELECT 
        op.ProductId,
        o.DateCreated,
        op.Quantity
    FROM dbo.[OrdersProducts] op
    INNER JOIN dbo.[Orders] o ON op.OrderId = o.Id
    """

    cursor.execute(query)
    rows = cursor.fetchall()
    conn.close()

    df = pd.DataFrame(rows)

    if df.empty:
        print("⚠️ Няма намерени продажби.")
        return pd.DataFrame()

    df['DateCreated'] = pd.to_datetime(df['DateCreated'], errors='coerce')
    df['Year'] = df['DateCreated'].dt.year
    df['Month'] = df['DateCreated'].dt.month

    grouped = df.groupby(['ProductId', 'Year', 'Month'])['Quantity'].sum().reset_index()

    return grouped

def train_model(df):
    X = df[['ProductId', 'Year', 'Month']]
    y = df['Quantity']

    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

    model = RandomForestRegressor(n_estimators=100, random_state=42)
    model.fit(X_train, y_train)

    y_pred = model.predict(X_test)

    rmse = mean_squared_error(y_test, y_pred, squared=False)
    print(f"✅ Обучението е завършено. RMSE: {rmse:.2f}")

    joblib.dump(model, 'sales_forecast_model.pkl')
    print("📦 Моделът е запазен в sales_forecast_model.pkl")

if __name__ == '__main__':
    df = get_sales_data()
    if not df.empty:
        train_model(df)
