Add-Type -AssemblyName System.Drawing

$bmp = New-Object System.Drawing.Bitmap(512, 512)
$g = [System.Drawing.Graphics]::FromImage($bmp)

# Ensure the entire canvas is filled with true 100% transparency first
$g.Clear([System.Drawing.Color]::Transparent)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Orb boundaries (16, 16, 480, 480)
$rect = New-Object System.Drawing.Rectangle(16, 16, 480, 480)

# Base cyan color
$cyanColor = [System.Drawing.Color]::FromArgb(255, 0, 180, 255)
$brush = New-Object System.Drawing.SolidBrush($cyanColor)
$g.FillEllipse($brush, $rect)

# Darker blue border
$penColor = [System.Drawing.Color]::FromArgb(255, 0, 100, 200)
$pen = New-Object System.Drawing.Pen($penColor, 12)
$g.DrawEllipse($pen, $rect)

# Glossy highlight to make it look like a bubble/orb
$highlightRect = New-Object System.Drawing.Rectangle(64, 64, 128, 128)
$highlightColor = [System.Drawing.Color]::FromArgb(180, 255, 255, 255)
$highlightBrush = New-Object System.Drawing.SolidBrush($highlightColor)
$g.FillEllipse($highlightBrush, $highlightRect)

# Clean up graphics object
$g.Dispose()

# Save over the problematic sprite
$savePath = "c:\Users\ayushi\splatAndSpan_germPatrol\Assets\Sprites\Collectibles\water_sprite.png"
$bmp.Save($savePath, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

Write-Host "Successfully generated true transparent water orb at $savePath"
