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

if __name__ == '__main__':
    df = get_sales_data()
    print(df.head())
