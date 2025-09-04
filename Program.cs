// See https://aka.ms/new-console-template for more information
using GitCleaner.Models;
using System.Diagnostics;

Console.WriteLine("GitCleaner");

//if (!System.IO.Directory.Exists(".git"))
//{
//    Console.WriteLine("This is not a git repository.");
//    return;
//}

Console.Write("Enter the primary branch (default: main): ");
string primaryBranch = Console.ReadLine();
if (string.IsNullOrWhiteSpace(primaryBranch))
{
    primaryBranch = "main";
}


while (true)
{
    Console.WriteLine("== Menu: ==");
    Console.WriteLine("1 - Check actual status of all branches");
    Console.WriteLine("2 - Sync all branches");
    Console.WriteLine("3 - Delete all branches that are already merged");
    Console.WriteLine("4 - Quit");
    Console.Write("Enter your choice: ");
    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            CheckStatus(primaryBranch);
            break;
        case "2":
            SyncBranches(primaryBranch);
            break;
        case "3":
            DeleteMergedBranches(primaryBranch);
            break;
        case "4":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    Console.WriteLine("");
    Console.WriteLine("");
}

void CheckStatus(string primaryBranch)
{
    List<Branch> branches = GetBranches();
    branches.Sort((b1, b2) => b2.CommitsAhead.CompareTo(b1.CommitsAhead));
    Console.WriteLine($"Name (<- CommitsBehind / CommitsAhead ->)");
    foreach (Branch branch in branches)
    {
        Console.WriteLine($"{branch.Name} (<-{branch.CommitsBehind} / {branch.CommitsAhead} ->)");
    }
}


void SyncBranches(string primaryBranch)
{
    List<Branch> branches = GetBranches();
    foreach (Branch branch in branches)
    {
        Console.WriteLine($"Syncing branch: {branch.Name}");
        Git($"checkout {branch.Name}");
        string output = Git("pull");
        Console.WriteLine(output);
    }
}


void DeleteMergedBranches(string primaryBranch)
{
    List<Branch> branches = GetBranches();
    Console.WriteLine($"You are going to delete {branches.Where(c => c.CommitsAhead == 0).Count()} branches");
    Console.WriteLine($"Are you sure? (Y/N)");
    string confirmation = Console.ReadLine();
    if (confirmation.ToUpper() != "Y")
    {
        Console.WriteLine("Aborting...");
        return;
    }

    Console.WriteLine($"Delete them also on remote? (Y/N)");
    bool remote = Console.ReadLine().ToUpper() != "Y";

    foreach (Branch branch in branches)
    {
        if (branch.CommitsAhead == 0)
        {
            Console.WriteLine($"Deleting branch: {branch.Name}");
            Git($"branch -d {branch.Name}");
            if (remote)
                Git($"push origin --delete {branch.Name}");
        }
    }
    Console.WriteLine($"{branches.Where(c => c.CommitsAhead == 0).Count()} branches deleted");
}


List<Branch> GetBranches()
{
    List<Branch> branches = new List<Branch>();
    string output = Git("branch");
    string[] lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    foreach (string line in lines)
    {
        string branchName = line.Trim();
        if (branchName.StartsWith("*"))
        {
            branchName = branchName.Substring(1).Trim();
        }
        if (branchName== primaryBranch) {
            continue; // Skip primary branch
        }
        string behindOutput = Git($"rev-list --count {branchName}..{primaryBranch}");
        string aheadOutput = Git($"rev-list --count {primaryBranch}..{branchName}");

        if (int.TryParse(aheadOutput.Trim(), out int commitsAhead) && int.TryParse(behindOutput.Trim(), out int commitsBehind))
        {
            branches.Add(new Branch
            {
                Name = branchName,
                CommitsAhead = commitsAhead,
                CommitsBehind = commitsBehind
            });
        }
        
    }

    return branches;

}

static string Git(string arguments)
{
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "git",
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process process = new Process { StartInfo = psi })
    {
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(errors))
        {
            output += Environment.NewLine + "ERRORS: " + errors;
        }

        return output;
    }
}

