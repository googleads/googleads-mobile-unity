-- This script is used to get the top Unity versions from the previous day. It is used to populate
-- the mobile_ads_sdk_ext.top_unity_versions table.

SET QueryRequest.accounting_group = 'mobile-ads-sdk-ext';
SET f1_instance = '/f1/query/prod';

-- Import macro to parse GET parameters:
RUN gws_tools_dremel_plx();

DEFINE MACRO decode_param decode(param(Request, $1));

DEFINE MACRO target_date DATE_SUB(CURRENT_DATE('America/Los_Angeles'), INTERVAL ${DAYS_AGO} DAY);

CREATE OR REPLACE TABLE mobile_ads_sdk_ext.top_unity_versions
AS (
  -- Read all data from existing table.
  SELECT *
  FROM mobile_ads_sdk_ext.top_unity_versions
  WHERE date != $target_date
  -- Append new data.
  UNION ALL
  SELECT
    $target_date AS date,
    $decode_param('v_unity') AS unity_version,
    COUNT(*) AS count,
    $decode_param('request_agent') AS request_agent
  FROM cafe.partnerlog.${DAYS_AGO}daysago
  WHERE $decode_param('v_unity') IS NOT NULL
  GROUP BY date, unity_version, request_agent
);
