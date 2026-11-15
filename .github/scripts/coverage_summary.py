#!/usr/bin/env python3
"""Summarise a Cobertura report as Markdown and enforce a minimum line coverage.

Usage: coverage_summary.py <report.cobertura.xml | directory> [--min-line PERCENT]

The Markdown goes to stdout (CI appends it to $GITHUB_STEP_SUMMARY).
Exit code 1 when line coverage is below --min-line, 2 when no report is found.
"""
import argparse
import glob
import os
import sys
import xml.etree.ElementTree as ET


def find_report(path):
    if os.path.isfile(path):
        return path
    reports = sorted(glob.glob(os.path.join(path, "**", "*.cobertura.xml"), recursive=True))
    return reports[-1] if reports else None


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("path")
    parser.add_argument("--min-line", type=float, default=0.0)
    args = parser.parse_args()

    report = find_report(args.path)
    if report is None:
        print(f"No Cobertura report found under {args.path}", file=sys.stderr)
        return 2

    root = ET.parse(report).getroot()
    line_rate = float(root.get("line-rate", 0)) * 100
    branch_rate = float(root.get("branch-rate", 0)) * 100
    covered = int(root.get("lines-covered", 0))
    valid = int(root.get("lines-valid", 0))

    # A source line can appear under several <class>/<method> elements, so de-duplicate per file.
    files = {}
    for cls in root.iter("class"):
        name = cls.get("filename", "?").replace("\\", "/")
        lines = files.setdefault(name, {})
        for line in cls.iter("line"):
            number = int(line.get("number"))
            lines[number] = max(lines.get(number, 0), int(line.get("hits", 0)))

    prefix = os.path.commonpath(list(files)) if files else ""
    print("## Code coverage")
    print()
    print(f"**Lines:** {line_rate:.1f}% ({covered}/{valid})  \n**Branches:** {branch_rate:.1f}%")
    if args.min_line:
        print(f"  \n**Minimum line coverage:** {args.min_line:.1f}%")
    print()
    print("| File | Lines | Covered |")
    print("|---|---:|---:|")
    for name, lines in sorted(files.items(), key=lambda item: (sum(1 for h in item[1].values() if h) / max(len(item[1]), 1), item[0])):
        hit = sum(1 for h in lines.values() if h)
        pct = 100 * hit / max(len(lines), 1)
        shown = (os.path.relpath(name, prefix) if prefix else name).replace(os.sep, "/")
        print(f"| `{shown}` | {len(lines)} | {pct:.0f}% |")

    if line_rate < args.min_line:
        print(f"\nLine coverage {line_rate:.1f}% is below the required {args.min_line:.1f}%.", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
