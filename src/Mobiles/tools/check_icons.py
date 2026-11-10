"""Check action-icon coverage and asset parity across the three mobile apps."""
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
rn = ROOT / 'ReactNative/src'
flutter = ROOT / 'Flutter/lib'
kotlin = ROOT / 'Kotlin/app/src/main/java/com/classifiedads/mobile'
maps = {
    'ReactNative': dict(re.findall(r'"([^"]+)": require\(".*?/action_(\w+)\.png"\)', (rn / 'shared/ui/action-icons.ts').read_text())),
    'Flutter': dict(re.findall(r"'([^']+)': '(\w+)'", (flutter / 'shared/widgets/action_label.dart').read_text())),
    'Kotlin': dict(re.findall(r'"([^"]+)" -> R.drawable.action_(\w+)', (kotlin / 'shared/ui/ActionLabel.kt').read_text())),
}
assert maps['ReactNative'] == maps['Flutter'] == maps['Kotlin'], 'Action icons differ between apps'
for icon in set(maps['ReactNative'].values()):
    assets = [ROOT / folder / f'action_{icon}.png' for folder in (
        'ReactNative/assets/icons', 'Flutter/assets/icons', 'Kotlin/app/src/main/res/drawable-nodpi')]
    assert len({p.read_bytes() for p in assets}) == 1, f'Icon asset differs: {icon}'

for name, root, extension, pattern in (
    ('ReactNative', rn, '*.tsx', r'<(?:HomeButton|Button)\b[^>]*?\btitle="([^"]+)"'),
    ('Flutter', flutter, '*.dart', r"ActionLabel\('([^']+)'"),
    ('Kotlin', kotlin, '*.kt', r'ActionLabel\("([^"]+)"'),
):
    for path in root.rglob(extension):
        source = path.read_text()
        labels = re.findall(pattern, source)
        # RecordPage forwards these labels to its shared action button.
        labels += re.findall(r'''createTitle\s*[:=]\s*["']([^"']+)["']''', source)
        # Dynamic text such as "Theme: dark" can select an explicit icon.
        if name == 'ReactNative':
            labels += re.findall(r'\bicon="([^"]+)"', source)
        for label in labels:
            assert label in maps[name], f'{path}: missing icon for {label}'
print(f'Checked {len(maps["ReactNative"])} action labels and identical assets across all three apps.')
