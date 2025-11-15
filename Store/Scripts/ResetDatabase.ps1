# Script để reset database
$dbPath = "bin/Debug/net9.0/store.db"

if (Test-Path $dbPath) {
    Remove-Item $dbPath -Force
    Write-Host "✓ Đã xóa database cũ: $dbPath" -ForegroundColor Green
} else {
    Write-Host "⚠ Database không tồn tại: $dbPath" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Hãy chạy lại ứng dụng để tạo database mới với schema đúng." -ForegroundColor Cyan
