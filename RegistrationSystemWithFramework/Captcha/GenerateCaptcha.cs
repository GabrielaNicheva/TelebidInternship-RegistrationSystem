using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace RegistrationSystemWithFramework.Captcha
{
    public class GenerateCaptcha
    {
        public string GetCaptchaText(int length = 6)
        {
            return GenerateRandomText(length);
        }

        public string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var captchaText = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                captchaText.Append(chars[random.Next(chars.Length)]);
            }

            return captchaText.ToString();
        }
        public Bitmap GenerateCaptchaImage(string captchaText)
        {
            int width = 250;
            int height = 80;
            var random = new Random();

            var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            var font = new Font("Arial", 30, FontStyle.Bold);

            Brush textBrush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Gray);

            float angle = random.Next(-10, 10);
            graphics.RotateTransform(angle);

            float x = random.Next(10, 40);
            float y = random.Next(10, 15);

            graphics.DrawString(captchaText, font, textBrush, x, y);

            graphics.RotateTransform(-angle);

            for (int i = 0; i < 10; i++)
            {
                int x1 = random.Next(width);
                int y1 = random.Next(height);
                int x2 = random.Next(width);
                int y2 = random.Next(height);

                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            return bitmap;
        }
    }
}