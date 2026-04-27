script_path=$(cd $(dirname $0) && pwd)
cd $script_path
pyinstaller src/main.py --noconfirm --onedir --name mediapipe-tracking-server