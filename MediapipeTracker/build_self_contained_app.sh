script_path=$(cd $(dirname $0) && pwd)
cd $script_path
path_to_put_in="../DemoApp/Assets/StreamingAssets"
pyinstaller src/main.py --noconfirm --onedir --name mediapipe-tracking-server --add-data "face_landmarker.task:." --distpath $path_to_put_in