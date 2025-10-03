node {
  id: "root"
  label: "Daily Unity Versions at 00:01"
  wait_for_schedule_node {
    cron_schedule: "1 0 * * *"
    timezone: "America/Los_Angeles"
  }
}
node {
  id: "script"
  label: "Top Unity Versions daily run"
  run_script_node {
    script_name {
      name_space: "google"
      name: "java_com_google_android_libraries_admob_demo_unity_googlemobileads_plx_prod_metrics_top_unity_versions"
    }
  }
}
edge {
  target: "script"
  source: "root"
}
authorize_as {
  scope: MDB_GROUP
  mdb_group {
    group_name: "mobile-ads-sdk-ext"
  }
}

enabled: true
variable {
  name: "DAYS_AGO"
  googlesql_expression: "'1'"
}
status_email {
  to_address: "mobile-ads-sdk-ext@google.com"
  cc_address: "vkini@google.com"
  condition: FAILURE
}
