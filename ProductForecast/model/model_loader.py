import joblib

MODEL_PATH = "sales_forecast_model.pkl"

def load_model():
    model = joblib.load(MODEL_PATH)
    return model
