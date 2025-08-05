FROM python:3.9-slim

RUN apt-get update && apt-get install -y cron && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY main.py forecast.py ./

RUN echo "0 0 * * * cd /app && python forecast.py > /proc/1/fd/1 2>/proc/1/fd/2" >> /etc/crontab
RUN chmod 0644 /etc/crontab
RUN crontab /etc/crontab

CMD ["/bin/bash", "-c", "cron && uvicorn main:app --host 0.0.0.0 --port 8000"]